using DasMulli.Win32.ServiceUtils;
using System;
using PeterKottas.DotNetCore.WindowsService.Enums;
using System.Diagnostics;
using System.ServiceProcess;
using PeterKottas.DotNetCore.CmdArgParser;
using System.Collections.Generic;
using System.Linq;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace PeterKottas.DotNetCore.WindowsService
{
    public static class ServiceRunner<SERVICE> where SERVICE : IMicroService
    {
        public static int Run(Action<HostConfigurator<SERVICE>> runAction)
        {
            var innerConfig = new HostConfiguration<SERVICE>();
            innerConfig.Action = ActionEnum.RunInteractive;
            innerConfig.Name = typeof(SERVICE).FullName;

            innerConfig.ExtraArguments = Parser.Parse(config =>
            {
                config.AddParameter(new CmdArgParam()
                {
                    Key = "username",
                    Description = "Username for the service account",
                    Value = val =>
                    {
                        innerConfig.Username = val;
                    }
                });
                config.AddParameter(new CmdArgParam()
                {
                    Key = "password",
                    Description = "Password for the service account",
                    Value = val =>
                    {
                        innerConfig.Password = val;
                    }
                });
                config.AddParameter(new CmdArgParam()
                {
                    Key = "name",
                    Description = "Service name",
                    Value = val =>
                    {
                        innerConfig.Name = val;
                    }
                });
                config.AddParameter(new CmdArgParam()
                {
                    Key = "description",
                    Description = "Service description",
                    Value = val =>
                    {
                        innerConfig.Description = val;
                    }
                });
                config.AddParameter(new CmdArgParam()
                {
                    Key = "displayName",
                    Description = "Service display name",
                    Value = val =>
                    {
                        innerConfig.DisplayName = val;
                    }
                });
                config.AddParameter(new CmdArgParam()
                {
                    Key = "action",
                    Description = "Installs the service. It's run like console application otherwise",
                    Value = val =>
                    {
                        switch (val)
                        {
                            case "install":
                                innerConfig.Action = ActionEnum.Install;
                                break;
                            case "start":
                                innerConfig.Action = ActionEnum.Start;
                                break;
                            case "stop":
                                innerConfig.Action = ActionEnum.Stop;
                                break;
                            case "uninstall":
                                innerConfig.Action = ActionEnum.Uninstall;
                                break;
                            case "run":
                                innerConfig.Action = ActionEnum.Run;
                                break;
                            case "run-interactive":
                                innerConfig.Action = ActionEnum.RunInteractive;
                                break;
                            default:
                                Console.WriteLine("{0} is unrecognized, will run the app as console application instead");
                                innerConfig.Action = ActionEnum.RunInteractive;
                                break;
                        }
                    }
                });

                config.UseDefaultHelp();
                config.UseAppDescription("Sample microservice application");
            });

            if (string.IsNullOrEmpty(innerConfig.Name))
            {
                innerConfig.Name = typeof(SERVICE).FullName;
            }

            if (string.IsNullOrEmpty(innerConfig.DisplayName))
            {
                innerConfig.DisplayName = innerConfig.Name;
            }

            if (string.IsNullOrEmpty(innerConfig.Description))
            {
                innerConfig.Description = "No description";
            }

            var hostConfiguration = new HostConfigurator<SERVICE>(innerConfig);

            try
            {
                runAction(hostConfiguration);
                innerConfig.Service = innerConfig.ServiceFactory();
                ConfigureService(innerConfig);
                return 0;
            }
            catch (Exception e)
            {
                Error(innerConfig, e);
                return -1;
            }
        }

        private static string GetServiceCommand(List<string> extraArguments)
        {
            var host = Process.GetCurrentProcess().MainModule.FileName;

            if (!host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                //For self-contained apps, skip the dll path
                extraArguments = extraArguments.Skip(1).ToList();
            }

            var fullServiceCommand = string.Format("{0} {1} {2}", host, string.Join(" ", extraArguments), "action:run");
            return fullServiceCommand;
        }

        private static void ConfigureService(HostConfiguration<SERVICE> config)
        {
            using (var sc = new ServiceController(config.Name))
            {

                switch (config.Action)
                {
                    case ActionEnum.Install:
                        Win32ServiceCredentials cred = Win32ServiceCredentials.LocalSystem;
                        if (!string.IsNullOrEmpty(config.Username))
                        {
                            cred = new Win32ServiceCredentials(config.Username, config.Password);
                        }
                        try
                        {
                            new Win32ServiceManager().CreateService(
                                config.Name,
                                config.DisplayName,
                                config.Description,
                                GetServiceCommand(config.ExtraArguments),
                                cred,
                                autoStart: true,
                                startImmediately: true,
                                errorSeverity: ErrorSeverity.Normal);
                            Console.WriteLine($@"Successfully registered and started service ""{config.Name}"" (""{config.Description}"")");
                        }
                        catch (Exception e)
                        {
                            if (!e.Message.Contains("already exists"))
                            {
                                throw;
                            }
                            Console.WriteLine($@"Service ""{config.Name}"" (""{config.Description}"") was already installed");
                        }
                        break;
                    case ActionEnum.Uninstall:
                        try
                        {
                            if(!(sc.Status==ServiceControllerStatus.Stopped||sc.Status==ServiceControllerStatus.StopPending))
                            {
                                sc.Stop();
                                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(1000));
                                Console.WriteLine($@"Successfully stopped service ""{config.Name}"" (""{config.Description}"")");
                            }
                            new Win32ServiceManager().DeleteService(config.Name);
                            Console.WriteLine($@"Successfully unregistered service ""{config.Name}"" (""{config.Description}"")");
                        }
                        catch (Exception e)
                        {
                            if (!e.Message.Contains("does not exist"))
                            {
                                throw;
                            }
                            Console.WriteLine($@"Service ""{config.Name}"" (""{config.Description}"") does not exist. No action taken.");
                        }
                        break;
                    case ActionEnum.Run:
                        var testService = new InnerService(config.Name, () => Start(config), () => Stop(config));
                        var serviceHost = new Win32ServiceHost(testService);
                        serviceHost.Run();
                        break;
                    case ActionEnum.RunInteractive:
                        Start(config);
                        break;
                    case ActionEnum.Stop:
                        if (!(sc.Status == ServiceControllerStatus.Stopped | sc.Status==ServiceControllerStatus.StopPending))
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(1000));
                            Console.WriteLine($@"Successfully stopped service ""{config.Name}"" (""{config.Description}"")");
                        }
                        else
                        {
                            Console.WriteLine($@"Service ""{config.Name}"" (""{config.Description}"") is already stopped or stop is pending.");
                        }
                        break;
                    case ActionEnum.Start:
                        if (!(sc.Status == ServiceControllerStatus.StartPending | sc.Status == ServiceControllerStatus.Running))
                        {
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(1000));
                            Console.WriteLine($@"Successfully started service ""{config.Name}"" (""{config.Description}"")");
                        }
                        else
                        {
                            Console.WriteLine($@"Service ""{config.Name}"" (""{config.Description}"") is already running or start is pending.");
                        }
                        break;
                }
            }
        }

        private static void Start(HostConfiguration<SERVICE> config)
        {
            try
            {
                config.OnServiceStart(config.Service, config.ExtraArguments);
            }
            catch (Exception e)
            {
                Error(config, e);
            }
        }

        private static void Stop(HostConfiguration<SERVICE> config)
        {
            try
            {
                config.OnServiceStop(config.Service);
            }
            catch (Exception e)
            {
                Error(config, e);
            }
        }

        private static void Error(HostConfiguration<SERVICE> config, Exception e = null)
        {
            config.OnServiceError(e);
        }
    }
}
