using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.ServiceContracts
{
    [ServiceContract(Name = "IUserSaverService", Namespace = "Server/")]
    interface IUserSaverService
    {
        [OperationContract]
        void RegisterUser(User user);
        [OperationContract]
        User GetUser(string email);
    }
}
