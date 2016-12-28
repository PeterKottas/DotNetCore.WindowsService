using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.Example
{
    public class ExampleService : IMicroService
    {
        public void Start()
        {
            Console.WriteLine("I started");
        }

        public void Stop()
        {
            Console.WriteLine("I stopped");
        }
    }
}
