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
    [ServiceContract(Name = "IUserSaverService", Namespace = "Server/")]
    interface IUserSaverService
    {
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void RegisterUser(User user);
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        User GetUser(string email, string machineName);
    }
}
