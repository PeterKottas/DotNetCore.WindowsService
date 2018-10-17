using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;

namespace PeterKottas.DotNetCore.WindowsService.Configurators.Service
{
    public class ServiceConfigurator<TService> where TService : IMicroService
    {
        private readonly HostConfiguration<TService> _config;

        public ServiceConfigurator(HostConfiguration<TService> config)
        {
            _config = config;
        }

        public void ServiceFactory(Func<List<string>, IMicroServiceController, TService> serviceFactory)
        {
            _config.ServiceFactory = serviceFactory;
        }

        public void OnStart(Action<TService, List<string>> onStart)
        {
            _config.OnServiceStart = onStart;
        }

        public void OnStop(Action<TService> onStop)
        {
            _config.OnServiceStop = onStop;
        }

        public void OnError(Action<Exception> onError)
        {
            _config.OnServiceError = onError;
        }

        public void OnPause(Action<TService> onPause)
        {
            _config.OnServicePause = onPause;
        }

        public void OnInstall(Action<TService> onInstall)
        {
            _config.OnServiceInstall = onInstall;
        }

        public void OnUnInstall(Action<TService> onUnInstall)
        {
            _config.OnServiceUnInstall = onUnInstall;
        }

        public void OnContinue(Action<TService> onContinue)
        {
            _config.OnServiceContinue = onContinue;
        }

        public void OnShutdown(Action<TService> onShutdown)
        {
            _config.OnServiceShutdown = onShutdown;
        }
    }
}