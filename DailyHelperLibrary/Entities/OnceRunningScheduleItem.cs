using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Scheduler;

namespace DailyHelperLibrary.Entities
{
    /// <summary>
    /// Entity that encapsulates configuration of task that runs once.
    /// It needs executable path and triggering time (day, month, time).
    /// Has also optional message property, that is showed when triggering time is come.
    /// </summary>
    public class OnceRunningScheduleItem
    {
        public Guid Id { get; private set; }
        public DateTime TriggeringTime { get; private set; }
        public string ExecutablePath { get; private set; }
        public string Message { get; set; }

        public OnceRunningScheduleItem(string executable, DateTime triggeringTime)
        {
            ExecutablePath = executable;
            TriggeringTime = triggeringTime;
            Id = Guid.NewGuid();
        }

        internal OnceRunningScheduleItem(InnerOnceRunningScheduleItem item)
        {
            Id = item.Id;
            TriggeringTime = item.TriggeringTime;
            ExecutablePath = item.ExecutablePath;
            Message = item.Message;
        }

        internal virtual InnerOnceRunningScheduleItem InnerScheduleItem
        {
            get
            {
                InnerOnceRunningScheduleItem item = new InnerOnceRunningScheduleItem
                {
                    Id = Id,
                    TriggeringTime = TriggeringTime,
                    ExecutablePath = ExecutablePath,
                    Message = Message
                };
                return item;
            }
        }
    }
}
