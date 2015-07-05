using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Savers;
using DailyHelperLibrary.ServiceContracts;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Proxies
{
    public class ScheduleItemSaver : IScheduleItemSaver, IDisposable
    {
        private ScheduleItemSaverProxy _proxy = new ScheduleItemSaverProxy();

        public void SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName)
        {
            _proxy.SaveScheduleItem(user, item, machineName);
        }

        public void DeleteScheduleItem(OnceRunningScheduleItem item)
        {
            _proxy.DeleteScheduleItem(item);
        }

        public Dictionary<Guid, OnceRunningScheduleItem> GetScheduleItems(User user, string machineName)
        {
            return _proxy.GetScheduleItems(user, machineName);
        }


        class ScheduleItemSaverProxy : ClientBase<IScheduleItemSaverService>
        {
            public ScheduleItemSaverProxy() :
                base("SaveScheduleItemEndpoint")
            { }

            public void SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName)
            {
                Channel.SaveScheduleItem(user.ServiceUser, item.ServiceScheduleItem, machineName);
            }

            public void DeleteScheduleItem(OnceRunningScheduleItem item)
            {
                Channel.DeleteScheduleItem(item.ServiceScheduleItem);
            }

            public Dictionary<Guid, OnceRunningScheduleItem> GetScheduleItems(User user, string machineName)
            {
                return Channel.GetScheduleItems(user.ServiceUser, machineName).ToDictionary(x => x.Key, x => x.Value.ScheduleItem);
            }
        }

        public void Dispose()
        {
            _proxy.Close();
        }
    }
}
