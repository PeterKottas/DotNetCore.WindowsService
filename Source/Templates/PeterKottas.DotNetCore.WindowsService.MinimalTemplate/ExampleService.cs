using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;

namespace PeterKottas.DotNetCore.WindowsService.MinimalTemplate
{
    public class ExampleService : IMicroService
    {
        private readonly IMicroServiceController _controller;
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

            if (_controller != null)
            {
                _controller.Stop();
            }
        }

        public void Stop()
        {
            File.AppendAllText(_fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }
    }
}
