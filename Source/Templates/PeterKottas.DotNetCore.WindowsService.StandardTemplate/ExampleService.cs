using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PeterKottas.DotNetCore.WindowsService.StandardTemplate
{
    public class ExampleService : MicroService, IMicroService
    {
        private IMicroServiceController _controller;
        private ILogger<ExampleService> _logger;

        public ExampleService()
        {
            _controller = null;
        }

        public ExampleService(IMicroServiceController controller, ILogger<ExampleService> logger)
        {
            _controller = controller;
            _logger = logger;
        }

        
        public void Start()
        {
            StartBase();
            Timers.Start("Poller", 1000, () =>
            {
            _logger.LogInformation(string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));
            });
            _logger.LogTrace("Started\n");
        }

        public void Stop()
        {
            StopBase();
            _logger.LogTrace("Stopped\n");
        }
    }
}
