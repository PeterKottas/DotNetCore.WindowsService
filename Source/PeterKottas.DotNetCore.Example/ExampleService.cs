using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Extensions.PlatformAbstractions;

namespace PeterKottas.DotNetCore.Example
{
    public class ExampleService : IMicroService
    {
        private string fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
        public void Start()
        {
            Console.WriteLine("I started");
            Console.WriteLine(fileName);
            File.AppendAllText(fileName, "Started\n");
        }

        public void Stop()
        {
            File.AppendAllText(fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }
    }
}
