using DasMulli.Win32.ServiceUtils;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class InnerService : IShutdownableWin32Service
    {
        private readonly string _serviceName;
        private readonly Action _onStart;
        private readonly Action _onStopped;
        private readonly Action _onShutdown;

        public InnerService(string serviceName, Action onStart, Action onStopped, Action onShutdown)
        {
            _serviceName = serviceName;
            _onStart = onStart;
            _onStopped = onStopped;
            _onShutdown = onShutdown;
        }

        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
        }

        public void Shutdown()
        {
            _onShutdown();
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            try
            {
                _onStart();
            }
            catch (Exception)
            {
                _onStopped();
                serviceStoppedCallback();
            }
        }

        public void Stop()
        {
            _onStopped();
        }
    }
}
