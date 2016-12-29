using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService.Base
{
    public class MicroService : IDisposable
    {
        protected Timers Timers { get; private set; }
        private bool disposed = false;

        public void StartBase()
        {
            Timers = new Timers();
        }

        public void StopBase()
        {
            Timers.Stop();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                StopBase();
            }

            disposed = true;
        }

        ~MicroService()
        {
            Dispose(false);
        }
    }
}
