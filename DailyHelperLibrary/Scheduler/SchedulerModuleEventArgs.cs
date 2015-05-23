using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Scheduler
{
    public class SchedulerModuleEventArgs<T>
        where T: OnceRunningScheduleItem
    {
        public T ScheduleItem { get; private set; }
        public User User { get; private set; }

        public SchedulerModuleEventArgs(T scheduleItem, User user)
        {
            ScheduleItem = scheduleItem;
            User = user;
        }
    }
}
