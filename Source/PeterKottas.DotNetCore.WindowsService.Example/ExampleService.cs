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

		private Timer _timer = new Timer(1000);

        public ExampleService()
        {
            _controller = null;
        }

        public ExampleService(IMicroServiceController controller)
        {
            _controller = controller;
        }

        private string fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
        public void Start()
        {
            Console.WriteLine("I started");
            Console.WriteLine(fileName);
            File.AppendAllText(fileName, "Started\n");

			_timer.Elapsed += _timer_Elapsed;
			_timer.Start();
        }

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			File.AppendAllText(fileName, string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));
		}

		public void Stop()
        {
			_timer.Stop();
            File.AppendAllText(fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }
    }
}
