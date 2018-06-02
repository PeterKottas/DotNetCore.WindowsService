using DasMulli.Win32.ServiceUtils;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class InnerService : IShutdownableWin32Service
    {
        string serviceName;
        Action onStart;
        Action onStopped;
        Action onShutdown;

        public InnerService(string serviceName, Action onStart, Action onStopped, Action onShutdown)
        {
            this.serviceName = serviceName;
            this.onStart = onStart;
            this.onStopped = onStopped;
            this.onShutdown = onShutdown;
        }

        public string ServiceName
        {
            get
            {
                return serviceName;
            }
        }

        public void Shutdown()
        {
            onShutdown();
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            try
            {
                onStart();
            }
            catch (Exception)
            {
                onStopped();
                serviceStoppedCallback();
            }
        }

        public void Stop()
        {
            onStopped();
        }
    }
}
