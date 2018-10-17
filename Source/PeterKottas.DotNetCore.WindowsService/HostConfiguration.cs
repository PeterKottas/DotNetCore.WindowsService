using System;
using System.Collections.Generic;
using PeterKottas.DotNetCore.WindowsService.Enums;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using DasMulli.Win32.ServiceUtils;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class HostConfiguration<TService> where TService : IMicroService
    {
        public HostConfiguration()
        {
            OnServiceStop = service => { };
            OnServiceShutdown = service => { };
            OnServiceError = e =>
            {
                Console.WriteLine(e.ToString());
            };
        }

        public ActionEnum Action { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public bool StartImmediately { get; set; } = true;

        public int ConsoleTimeout { get; set; } = 150;

        public int ServiceTimeout { get; set; } = 1500;

        public Win32ServiceCredentials DefaultCred { get; set; } = Win32ServiceCredentials.LocalSystem;

        public TService Service { get; set; }

        public Func<List<string>, IMicroServiceController, TService> ServiceFactory { get; set; }

        public Action<TService, List<string>> OnServiceStart { get; set; }

        public Action<TService> OnServiceStop { get; set; }

        public Action<TService> OnServiceInstall { get; set; }

        public Action<TService> OnServiceUnInstall { get; set; }

        public Action<TService> OnServicePause { get; set; }

        public Action<TService> OnServiceContinue { get; set; }

        public Action<TService> OnServiceShutdown { get; set; }

        public Action<Exception> OnServiceError { get; set; }

        public List<string> ExtraArguments { get; set; }
    }
}
