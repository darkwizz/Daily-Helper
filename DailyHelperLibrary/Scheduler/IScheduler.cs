using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Scheduler
{
    public interface IScheduler: IDisposable
    {
        void SaveUserConfig(User user);
        /// <summary>
        /// Places passed <code>ScheduleItem item</code> into scheduling. When <code>item</code>
        /// is placed it's no need to run it explicitly.
        /// </summary>
        /// <param name="item">passed ScheduleItem</param>
        void PlaceOnScheduling(OnceRunningScheduleItem item);
        Dictionary<Guid, OnceRunningScheduleItem> LoadUserConfig(string login);
        /// <summary>
        /// Removes <code>ScheduleItem</code> from scheduler
        /// </summary>
        /// <param name="item">Removed <code>ScheduleItem></code></param>
        /// <returns><code>false</code>, if no such <code>ScheduleItem</code></returns>
        bool RemoveFromScheduling(OnceRunningScheduleItem item);
    }
}
