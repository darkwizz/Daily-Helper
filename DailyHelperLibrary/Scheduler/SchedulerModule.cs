using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Scheduler
{
    public class SchedulerModule
    {
        private IScheduler _scheduler;

        public SchedulerModule(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public EventResult OnOnceRunningSelected(SchedulerModuleEventArgs<OnceRunningScheduleItem> e)
        {
            PlaceOnScheduling(e.User, e.ScheduleItem);
            return new EventResult(true);
        }

        public EventResult OnRegularlyRunningSelected(SchedulerModuleEventArgs<RegularlyRunningScheduleItem> e)
        {
            PlaceOnScheduling(e.User, e.ScheduleItem);
            return new EventResult(true);
        }

        private void PlaceOnScheduling(User user, OnceRunningScheduleItem item)
        {
            user.ScheduleItems.Add(item.Id, item);
            _scheduler.PlaceOnScheduling(item); // this schedule item is placed on schedule immediately
        }

        /// <summary>
        /// Removes <code>ScheduleItem</code> from scheduler. If no such task, then it'll be removed from user's list and
        /// it'll show error message
        /// </summary>
        /// <param name="e">Deleted <code>ScheduleItem</code> info</param>
        /// <returns>true <code>EventResult</code>, if already added task is removed</returns>
        public EventResult OnDeletedScheduleItem(SchedulerModuleEventArgs<OnceRunningScheduleItem> e)
        {
            OnceRunningScheduleItem item = e.ScheduleItem;
            User user = e.User;

            bool success = _scheduler.RemoveFromScheduling(item);
            user.ScheduleItems.Remove(item.Id);
            if (!success)
            {
                return new EventResult(false, "No such task on scheduling. Please, place it on scheduler");
            }

            return new EventResult(true);
        }
    }
}
