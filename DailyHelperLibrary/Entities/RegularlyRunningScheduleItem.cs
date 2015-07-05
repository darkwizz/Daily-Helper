using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Scheduler;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Entities
{
    /// <summary>
    /// A class for configuration of scheduled task. Needs executable path,
    /// days of week and concrete time. Has also optional message property,
    /// that is showed every time when time is come.
    /// </summary>
    public class RegularlyRunningScheduleItem: OnceRunningScheduleItem
    {
        private const int DAYS_COUNT = 7;

        /// <summary>
        /// Gets or sets running days bits. RunningDays[0] => Monday,
        /// RunningDays[1] => Tuesday etc.
        /// </summary>
        public bool[] RunningDays { get; private set; }

        public RegularlyRunningScheduleItem(string executable, DateTime triggeringTime) :
            base(executable, triggeringTime)
        {
            RunningDays = new bool[DAYS_COUNT]; // all items has default false value
        }

        internal RegularlyRunningScheduleItem(ServiceRegularlyRunningScheduleItem item) :
            base(item)
        {
            RunningDays = item.RunningDays;
        }

        internal override ServiceOnceRunningScheduleItem ServiceScheduleItem
        {
            get
            {
                ServiceRegularlyRunningScheduleItem item = new ServiceRegularlyRunningScheduleItem
                {
                    Id = Id,
                    TriggeringTime = TriggeringTime,
                    ExecutablePath = ExecutablePath,
                    Message = Message,
                    RunningDays = RunningDays
                };
                return item;
            }
        }
    }
}
