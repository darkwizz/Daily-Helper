using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Savers
{
    public interface IScheduleItemSaver
    {
        void SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName);
        void DeleteScheduleItem(OnceRunningScheduleItem item);
        Dictionary<Guid, OnceRunningScheduleItem> GetScheduleItems(User user, string machineName);
    }
}
