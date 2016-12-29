using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;

namespace PeterKottas.DotNetCore.WindowsService.Configurators.Service
{
    public class ServiceConfigurator<SERVICE> where SERVICE : IMicroService
    {
        private HostConfiguration<SERVICE> config;
        public ServiceConfigurator(HostConfiguration<SERVICE> config)
        {
            this.config = config;
        }

        public void ServiceFactory(Func<List<string>, SERVICE> serviceFactory)
        {
            config.ServiceFactory = serviceFactory;
        }

        public void OnStart(Action<SERVICE, List<string>> onStart)
        {
            config.OnServiceStart = onStart;
        }

        public void OnStop(Action<SERVICE> onStop)
        {
            config.OnServiceStop = onStop;
        }

        public void OnError(Action<Exception> onError)
        {
            config.OnServiceError = onError;
        }
    }
}