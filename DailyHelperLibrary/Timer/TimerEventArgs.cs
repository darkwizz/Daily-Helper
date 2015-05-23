using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyHelperLibrary.Timer
{
    public class TimerEventArgs: EventArgs
    {
        public TimerCallback Tick { get; set; }
        public TimeSpan Time { get; private set; }

        public TimerEventArgs(int hours, int minutes, int seconds)
        {
            Time = new TimeSpan(hours, minutes, seconds);
        }

        public TimerEventArgs(int minutes, int seconds) :
            this(0, minutes, seconds)
        { }

        public TimerEventArgs(int seconds) :
            this(0, 0, seconds)
        { }

        public TimerEventArgs(TimeSpan time)
        {
            Time = time;
        }
    }
}
