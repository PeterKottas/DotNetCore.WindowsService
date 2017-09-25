using DasMulli.Win32.ServiceUtils;
using System;
using PeterKottas.DotNetCore.WindowsService.Enums;
using System.Diagnostics;
using System.ServiceProcess;
using PeterKottas.DotNetCore.CmdArgParser;
using System.Collections.Generic;
using System.Linq;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Threading.Tasks;

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
                if (innerConfig.Action == ActionEnum.Run || innerConfig.Action == ActionEnum.RunInteractive)
                {
                    var controller = new MicroServiceController(
                        () =>
                        {
                            var task = Task.Factory.StartNew(() =>
                            {
                                UsingServiceController(innerConfig, (sc, cfg) => StopService(cfg, sc));
                            });
                            //task.Wait();
                        }
                    );
                    innerConfig.Service = innerConfig.ServiceFactory(innerConfig.ExtraArguments, controller);
                }
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
            if (host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                var appPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    PlatformServices.Default.Application.ApplicationName + ".dll");
                host = string.Format("{0} \"{1}\"", host, appPath);
            }
            if (!host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                //For self-contained apps, skip the dll path
                extraArguments = extraArguments.Skip(1).ToList();
            }

            var fullServiceCommand = string.Format("{0} {1} {2}", host, string.Join(" ", extraArguments), "action:run");
            return fullServiceCommand;
        }

        private static void Install(HostConfiguration<SERVICE> config, ServiceController sc, int counter = 0)
        {
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
                if (e.Message.Contains("already exists"))
                {
                    Console.WriteLine($@"Service ""{config.Name}"" (""{config.Description}"") was already installed. Reinstalling...");
                    Reinstall(config, sc);
                }
                else if (e.Message.Contains("The specified service has been marked for deletion"))
                {
                    if (counter < 10)
                    {
                        System.Threading.Thread.Sleep(500);
                        counter++;
                        string suffix = "th";
                        if (counter == 1)
                        {
                            suffix = "st";
                        }
                        else if (counter == 2)
                        {
                            suffix = "nd";
                        }
                        else if (counter == 3)
                        {
                            suffix = "rd";
                        }
                        Console.WriteLine("The specified service has been marked for deletion. Retrying {0}{1} time", counter, suffix);
                        Install(config, sc, counter);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        private static void Uninstall(HostConfiguration<SERVICE> config, ServiceController sc)
        {
            try
            {
                if (!(sc.Status == ServiceControllerStatus.Stopped || sc.Status == ServiceControllerStatus.StopPending))
                {
                    StopService(config, sc);
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
        }

        private static void StopService(HostConfiguration<SERVICE> config, ServiceController sc)
        {
            if (!(sc.Status == ServiceControllerStatus.Stopped | sc.Status == ServiceControllerStatus.StopPending))
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(1000));
                Console.WriteLine($@"Successfully stopped service ""{config.Name}"" (""{config.Description}"")");
            }
            else
            {
                Console.WriteLine($@"Service ""{config.Name}"" (""{config.Description}"") is already stopped or stop is pending.");
            }
        }

        private static void StartService(HostConfiguration<SERVICE> config, ServiceController sc)
        {
            if (!(sc.Status == ServiceControllerStatus.StartPending | sc.Status == ServiceControllerStatus.Running))
            {
                Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath);
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(1000));
                Console.WriteLine($@"Successfully started service ""{config.Name}"" (""{config.Description}"")");
            }
            else
            {
                Console.WriteLine($@"Service ""{config.Name}"" (""{config.Description}"") is already running or start is pending.");
            }
        }

        private static void Reinstall(HostConfiguration<SERVICE> config, ServiceController sc)
        {
            StopService(config, sc);
            Uninstall(config, sc);
            Install(config, sc);
        }

        private static void ConfigureService(HostConfiguration<SERVICE> config)
        {
            switch (config.Action)
            {
                case ActionEnum.Install:
                    UsingServiceController(config, (sc, cfg) => Install(cfg, sc));
                    break;
                case ActionEnum.Uninstall:
                    UsingServiceController(config, (sc, cfg) => Uninstall(cfg, sc));
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
                    UsingServiceController(config, (sc, cfg) => StopService(cfg, sc));
                    break;
                case ActionEnum.Start:
                    UsingServiceController(config, (sc, cfg) => StartService(cfg, sc));
                    break;
            }
        }

        private static void UsingServiceController(HostConfiguration<SERVICE> config, Action<ServiceController, HostConfiguration<SERVICE>> action)
        {
            using (var sc = new ServiceController(config.Name))
            {
                action(sc, config);
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
