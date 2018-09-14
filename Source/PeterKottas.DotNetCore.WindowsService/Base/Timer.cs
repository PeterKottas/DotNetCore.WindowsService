using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeterKottas.DotNetCore.WindowsService.Base
{
    /// <summary>
    /// Betfred timer
    /// </summary>
    public class Timer
    {
        private Thread thread;
        private AutoResetEvent stopRequest;
        private bool running = true;
        private bool paused = false;

        public Action OnTimer { get; set; }

        public Func<Task> OnTimerAsync { get; set; }

        public Action<Exception> OnException { get; set; }

        public string Name { get; private set; }

        public int Interval { get; set; }

        public Timer(string name, int interval, Func<Task> onTimerAsync, Action<Exception> onException = null)
        {
            this.OnTimer = null;
            this.OnTimerAsync = onTimerAsync == null ? default(Func<Task>) : onTimerAsync; ;
            this.Name = name;
            this.Interval = interval;
            this.OnException = onException == null ? (e) => { } : onException;
        }

        public Timer(string name, int interval, Action onTimer, Action<Exception> onException = null)
        {
            this.OnTimer = onTimer == null ? () => { } : onTimer; ;
            this.OnTimerAsync = null;
            this.Name = name;
            this.Interval = interval;
            this.OnException = onException == null ? (e) => { } : onException;
        }

        private async void InternalWork(object arg)
        {
            while (running)
            {
                try
                {
                    if (!paused)
                    {
                        if (this.OnTimer != null)
                            this.OnTimer();
                        else
                            await this.OnTimerAsync();
                    }
                }
                catch (Exception exception)
                {
                    this.OnException(exception);
                }

                try
                {
                    if (stopRequest.WaitOne(Interval))
                    {
                        return;
                    }
                }
                catch (Exception exception)
                {
                    this.OnException(exception);
                }

            }
        }

        public void Start()
        {
            stopRequest = new AutoResetEvent(false);
            running = true;
            thread = new Thread(InternalWork);
            thread.Start();
        }

        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false;
        }

        public void Stop()
        {
            if (running)
            {
                running = false;
                stopRequest.Set();
                thread.Join();

                thread = null;
                stopRequest.Dispose();
                stopRequest = null;
            }
        }
    }
}
