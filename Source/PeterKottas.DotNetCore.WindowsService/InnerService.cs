using DasMulli.Win32.ServiceUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class InnerService : IWin32Service
    {
        string serviceName;
        Action onStart;
        Action onStopped;

        public InnerService(string serviceName, Action onStart, Action onStopped)
        {
            this.serviceName = serviceName;
            this.onStart = onStart;
            this.onStopped = onStopped;
        }

        public string ServiceName
        {
            get
            {
                return serviceName;
            }
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
