using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;
using System.Timers;

namespace PeterKottas.DotNetCore.WindowsService.Example
{
    public class ExampleService : IMicroService
    {
        private IMicroServiceController _controller;

		private readonly Timer _timer = new Timer(1000);
        private readonly string _fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");

        public ExampleService()
        {
            _controller = null;
        }

        public ExampleService(IMicroServiceController controller)
        {
            _controller = controller;
        }

        public void Start()
        {
            Console.WriteLine("I started");
            Console.WriteLine(_fileName);
            File.AppendAllText(_fileName, "Started\n");

            /**
             * A timer is a simple example. But this could easily 
             * be a port or messaging queue client
             */ 
			_timer.Elapsed += _timer_Elapsed;
			_timer.Start();
        }

		public void Stop()
        {
			_timer.Stop();
            File.AppendAllText(_fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            File.AppendAllText(_fileName, string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));
        }
    }
}
