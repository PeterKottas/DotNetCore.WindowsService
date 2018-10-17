using System;
using System.Collections.Generic;
using System.Linq;

namespace PeterKottas.DotNetCore.WindowsService.Base
{
    public class Timers
    {
        private readonly List<Timer> _timers = new List<Timer>();

        public void Start(string timerName, int interval, Action onTimer, Action<Exception> onException = null)
        {
            var tmpTimer = _timers.FirstOrDefault(x => x.Name == timerName);

            if (tmpTimer == null)
            {
                tmpTimer = new Timer(timerName, interval, onTimer, onException);
                _timers.Add(tmpTimer);

                tmpTimer.Start();
            }
            else
            {
                tmpTimer.Stop();
                Update(timerName, interval, onTimer, onException);
                tmpTimer.Start();
            }
        }

        public void Update(string timerName, int interval = 0, Action onTimer = null, Action<Exception> onException = null)
        {
            var tmpTimer = _timers.FirstOrDefault(x => x.Name == timerName);

            if (tmpTimer == null)
                return;

            if (onTimer != null)
            {
                tmpTimer.OnTimer = onTimer;
            }
            if (onException != null)
            {
                tmpTimer.OnException = onException;
            }
            if (interval > 0 && interval != tmpTimer.Interval)
            {
                tmpTimer.Interval = interval;
            }
        }

        public void Resume()
        {
            foreach (var timer in _timers)
            {
                timer.Resume();
            }
        }

        public void Resume(string timerName)
        {
            var tmpTimer = _timers.FirstOrDefault(x => x.Name == timerName);

            if (tmpTimer != null)
            {
                tmpTimer.Resume();
            }
        }

        public void Pause()
        {
            foreach (var timer in _timers)
            {
                timer.Pause();
            }
        }

        public void Pause(string timerName)
        {
            var tmpTimer = _timers.FirstOrDefault(x => x.Name == timerName);

            if (tmpTimer != null)
            {
                tmpTimer.Pause();
            }
        }

        public void Stop()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
            }
        }

        public void Stop(string timerName)
        {
            var tmpTimer = _timers.FirstOrDefault(x => x.Name == timerName);

            if (tmpTimer != null)
            {
                tmpTimer.Stop();
            }
        }
    }
}
