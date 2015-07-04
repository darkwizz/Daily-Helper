using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;
using Server.Faults;

namespace Server.ServiceContracts
{
    [ServiceContract(Name = "IScheduleItemSaverService", Namespace = "Server/")]
    interface IScheduleItemSaverService
    {
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName);
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void DeleteScheduleItem(OnceRunningScheduleItem item);
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        Dictionary<Guid, OnceRunningScheduleItem> GetScheduleItems(User user, string machineName);
    }
}
