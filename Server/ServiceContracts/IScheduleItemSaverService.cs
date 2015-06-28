using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.ServiceContracts
{
    [ServiceContract(Name = "IScheduleItemSaverService", Namespace = "Server/")]
    interface IScheduleItemSaverService
    {
        [OperationContract]
        void SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName);
        [OperationContract]
        void DeleteScheduleItem(OnceRunningScheduleItem item);
    }
}
