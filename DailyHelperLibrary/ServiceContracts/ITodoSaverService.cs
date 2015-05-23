using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.ServiceContracts
{
    [ServiceContract(Name = "ITodoSaverService", Namespace = "Server/")]
    interface ITodoSaverService
    {
        [OperationContract]
        void SaveTodoItem(ServiceUser user, ServiceTodoItem item);
        [OperationContract]
        void RemoveTodoItem(ServiceUser user, ServiceTodoItem item);
    }
}
