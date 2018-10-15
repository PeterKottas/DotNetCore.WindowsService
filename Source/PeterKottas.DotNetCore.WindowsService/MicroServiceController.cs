using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class MicroServiceController : IMicroServiceController
    {
        private readonly Action _stop;

        public MicroServiceController(Action stop)
        {
            _stop = stop;
        }

        public void Stop()
        {
            _stop();
        }
    }
}
