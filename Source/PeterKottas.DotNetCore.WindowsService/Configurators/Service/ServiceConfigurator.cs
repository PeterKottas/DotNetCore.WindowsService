using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;

namespace PeterKottas.DotNetCore.WindowsService.Configurators.Service
{
    public class ServiceConfigurator<TService>
    {
        private HostConfiguration<TService> config;
        public ServiceConfigurator(HostConfiguration<TService> config)
        {
            this.config = config;
        }

        public void ServiceFactory(Func<List<string>, IMicroServiceController, TService> serviceFactory)
        {
            config.ServiceFactory = serviceFactory;
        }

        public void OnStart(Action<TService, List<string>> onStart)
        {
            config.OnServiceStart = onStart;
        }

        public void OnStop(Action<TService> onStop)
        {
            config.OnServiceStop = onStop;
        }

        public void OnError(Action<Exception> onError)
        {
            config.OnServiceError = onError;
        }

        public void OnPause(Action<TService> onPause)
        {
            config.OnServicePause = onPause;
        }

        public void OnInstall(Action<TService> onInstall)
        {
            config.OnServiceInstall = onInstall;
        }

        public void OnUnInstall(Action<TService> onUnInstall)
        {
            config.OnServiceUnInstall = onUnInstall;
        }

        public void OnContinue(Action<TService> onContinue)
        {
            config.OnServiceContinue = onContinue;
        }

        public void OnShutdown(Action<TService> onShutdown)
        {
            config.OnServiceShutdown = onShutdown;
        }

        public void OnCustomCommand(Action<TService, int> onCustomCommand) {
            config.OnServiceCustomCommand = onCustomCommand;
        }
    }
}