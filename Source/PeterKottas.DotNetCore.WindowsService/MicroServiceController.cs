using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
