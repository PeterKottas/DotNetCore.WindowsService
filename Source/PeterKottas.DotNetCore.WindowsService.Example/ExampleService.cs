using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;
using System.Timers;

namespace PeterKottas.DotNetCore.WindowsService.Example
{
    public class ExampleService : IMicroService
    {
        private IMicroServiceController controller;

		private Timer timer = new Timer(1000);

        public ExampleService()
        {
            controller = null;
        }

        public ExampleService(IMicroServiceController controller)
        {
            this.controller = controller;
        }

        private string fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
        public void Start()
        {
            Console.WriteLine("I started");
            Console.WriteLine(fileName);
            File.AppendAllText(fileName, "Started\n");

            /**
             * A timer is a simple example. But this could easily 
             * be a port or messaging queue client
             */ 
			timer.Elapsed += _timer_Elapsed;
			timer.Start();
        }

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			File.AppendAllText(fileName, string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));
		}

		public void Stop()
        {
			timer.Stop();
            File.AppendAllText(fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }
    }
}
