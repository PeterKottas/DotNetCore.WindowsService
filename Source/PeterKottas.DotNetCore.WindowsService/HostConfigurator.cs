using System;
using PeterKottas.DotNetCore.WindowsService.Configurators.Service;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class HostConfigurator<TService> where TService : IMicroService
    {
        private readonly HostConfiguration<TService> _innerConfig;

        public HostConfigurator(HostConfiguration<TService> innerConfig)
        {
            _innerConfig = innerConfig;
        }

        public void SetName(string serviceName, bool force = false)
        {
            if (!string.IsNullOrEmpty(_innerConfig.Name) || force)
            {
                _innerConfig.Name = serviceName;
            }
        }

        public void SetDisplayName(string displayName, bool force = false)
        {
            if (!string.IsNullOrEmpty(_innerConfig.DisplayName) || force)
            {
                _innerConfig.DisplayName = displayName;
            }
        }

        public void SetDescription(string description, bool force = false)
        {
            if (!string.IsNullOrEmpty(_innerConfig.Description) || force)
            {
                _innerConfig.Description = description;
            }
        }

        public void SetConsoleTimeout(int milliseconds)
        {
            _innerConfig.ConsoleTimeout = milliseconds;
        }

        public void SetServiceTimeout(int milliseconds)
        {
            _innerConfig.ServiceTimeout = milliseconds;
        }

        public string GetDefaultName()
        {
            return _innerConfig.Name;
        }

        public bool IsNameNullOrEmpty
        {
            get
            {
                return string.IsNullOrEmpty(_innerConfig.Name);
            }
        }

        public bool IsDescriptionNullOrEmpty
        {
            get
            {
                return string.IsNullOrEmpty(_innerConfig.Description);
            }
        }

        public bool IsDisplayNameNullOrEmpty
        {
            get
            {
                return string.IsNullOrEmpty(_innerConfig.DisplayName);
            }
        }

        public void Service(Action<ServiceConfigurator<TService>> serviceConfigAction)
        {
            try
            {
                var serviceConfig = new ServiceConfigurator<TService>(_innerConfig);

                serviceConfigAction(serviceConfig);

                if (_innerConfig.ServiceFactory == null)
                {
                    throw new ArgumentException("It's necessary to configure action that creates the service (ServiceFactory)");
                }

                if (_innerConfig.OnServiceStart == null)
                {
                    throw new ArgumentException("It's necessary to configure action that is called when the service starts");
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Configuring the service thrown an exception", e);
            }
        }
    }
}
