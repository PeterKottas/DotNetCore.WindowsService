using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Extensions.PlatformAbstractions;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService.Example
{
    public class ExampleService : IMicroService
    {
        private IMicroServiceController controller;

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
            if (controller != null)
            {
                controller.Stop();
            }
        }

        public void Stop()
        {
            File.AppendAllText(fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }
    }
}
