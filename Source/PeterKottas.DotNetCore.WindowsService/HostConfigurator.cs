using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PeterKottas.DotNetCore.WindowsService.Configurators.Service;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class HostConfigurator<SERVICE> where SERVICE : IMicroService
    {
        HostConfiguration<SERVICE> innerConfig;
        public HostConfigurator(HostConfiguration<SERVICE> innerConfig)
        {
            this.innerConfig = innerConfig;
        }

        public void SetName(string serviceName)
        {
            innerConfig.Name = serviceName;
        }

        public string GetDefaultName()
        {
            return innerConfig.Name;
        }

        public void Service(Action<ServiceConfigurator<SERVICE>> serviceConfigAction)
        {
            try
            {
                var serviceConfig = new ServiceConfigurator<SERVICE>(innerConfig);
                serviceConfigAction(serviceConfig);
                if(innerConfig.ServiceFactory==null)
                {
                    throw new ArgumentException("It's necesarry to configure action that creates the service (ServiceFactory)");
                }

                if (innerConfig.OnServiceStart == null)
                {
                    throw new ArgumentException("It's necesarry to configure action that is called when the service starts");
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Configuring the service thrown an exception", e);
            }
        }
    }
}
