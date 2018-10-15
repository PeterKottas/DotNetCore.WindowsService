using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;

namespace PeterKottas.DotNetCore.WindowsService.Example
{
	public class ExampleServiceTimer : MicroService, IMicroService
    {
        private IMicroServiceController controller;
        private readonly string _fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");

        public ExampleServiceTimer()
        {
            controller = null;
        }

        public ExampleServiceTimer(IMicroServiceController controller)
        {
            this.controller = controller;
        }

        public void Start()
        {
            StartBase();
            Timers.Start("Poller", 1000, () =>
            {
                File.AppendAllText(_fileName, string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));
            });
            Console.WriteLine("I started");
            File.AppendAllText(_fileName, "Started\n");
        }

        public void Stop()
        {
            StopBase();
            File.AppendAllText(_fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }
    }
}
