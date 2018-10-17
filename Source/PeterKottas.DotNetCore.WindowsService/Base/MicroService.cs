using System;

namespace PeterKottas.DotNetCore.WindowsService.Base
{
    public class MicroService : IDisposable
    {
        protected Timers Timers { get; private set; }
        private bool _disposed;

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
            if (_disposed)
                return;

            if (disposing)
            {
                StopBase();
            }

            _disposed = true;
        }

        ~MicroService()
        {
            Dispose(false);
        }
    }
}
