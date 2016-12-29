using System;
using System.IO;
using System.Diagnostics;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace PeterKottas.DotNetCore.Example
{
    public class ExampleServiceTimer : MicroService, IMicroService
    {
        private string fileName = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "log.txt");
        public void Start()
        {
            this.StartBase();
            Timers.Start("Poller", 1000, () =>
            {
                File.AppendAllText(fileName, string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));
            });
            Console.WriteLine("I started");
            File.AppendAllText(fileName, "Started\n");
        }

        public void Stop()
        {
            this.StopBase();
            File.AppendAllText(fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }
    }
}
