using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;

namespace PeterKottas.DotNetCore.WindowsService
{
    public class MicroServiceController : IMicroServiceController
    {
        private Action stop;

        public MicroServiceController(Action stop)
        {
            this.stop = stop;
        }

        public void Stop()
        {
            stop();
        }
    }
}
