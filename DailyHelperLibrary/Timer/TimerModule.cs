using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyHelperLibrary.Timer
{
    public class TimerModule
    {
        public EventResult OnTimerStarted(TimerEventArgs e)
        {
            TimerCallback callback = e.Tick;
            //callback += ClearTimer;
            long timerDuration = (long)e.Time.TotalMilliseconds;
            System.Threading.Timer timer = new System.Threading.Timer(callback, null, timerDuration, -1);
            //timer.Dispose(
            return new EventResult(true);
        }

        private void ClearTimer(object state)
        {
            System.Threading.Timer timer = state as System.Threading.Timer;
            if (timer == null)
            {
                // some error handling
            }
            timer.Dispose();
        }
    }
}
