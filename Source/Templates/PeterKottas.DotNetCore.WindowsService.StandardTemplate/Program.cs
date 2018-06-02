using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PeterKottas.DotNetCore.WindowsService.StandardTemplate
{
    class Program
    {
        public static void Main(string[] args)
        {
#if !DEBUG
            var configuration = new ConfigurationBuilder()
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
#else
            var configuration = new ConfigurationBuilder()
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .Build();
#endif

            var svcProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder
                    .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace)
                    .AddProvider(new LogFileProvider()); // Implemented vanilla LogFile provider but is easily swapped for Nlog or SeriLog (et al.) providers

                })
                .AddOptions()
                .AddSingleton(new LoggerFactory()
                .AddConsole())
                .BuildServiceProvider();

            var _logger = svcProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();


            ServiceRunner<ExampleService>.Run(config =>
            {
                var name = config.GetDefaultName();
                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) =>
                    {
                        return new ExampleService(controller, svcProvider.GetRequiredService<ILoggerFactory>().CreateLogger<ExampleService>());
                    });

                    serviceConfig.OnStart((service, extraParams) =>
                    {
                        _logger.LogTrace("Service {0} started", name);
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        _logger.LogTrace("Service {0} stopped", name);
                        service.Stop();
                    });

                    serviceConfig.OnError(e =>
                    {
                        _logger.LogError(e, string.Format("Service {0} errored with exception", name));
                    });
                });
            });
        }
    }
}
