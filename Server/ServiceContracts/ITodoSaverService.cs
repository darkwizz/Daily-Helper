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
    [ServiceContract(Name = "ITodoSaverService", Namespace = "Server/")]
    interface ITodoSaverService
    {
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void SaveTodoItem(User user, TodoItem item);
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void RemoveTodoItem(TodoItem item);
    }
}
