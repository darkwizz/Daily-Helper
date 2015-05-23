using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.ServiceContracts
{
    [ServiceContract(Name = "ITodoSaverService", Namespace = "Server/")]
    interface ITodoSaverService
    {
        [OperationContract]
        void SaveTodoItem(User user, TodoItem item);
        [OperationContract]
        void RemoveTodoItem(User user, TodoItem item);
    }
}
