using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.ServiceContracts
{
    [ServiceContract(Name = "IScheduleItemSaverService", Namespace = "Server/")]
    interface IScheduleItemSaverService
    {
        [OperationContract]
        void SaveScheduleItem(ServiceUser user, ServiceOnceRunningScheduleItem item, string machineName);
        [OperationContract]
        void DeleteScheduleItem(ServiceOnceRunningScheduleItem item);
    }
}
