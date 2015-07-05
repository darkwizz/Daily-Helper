using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.ServiceContracts
{
    [ServiceContract(Name = "IUserSaverService", Namespace = "Server/")]
    interface IUserSaverService
    {
        [OperationContract]
        void RegisterUser(ServiceUser user);
        [OperationContract]
        ServiceUser GetUser(string email);
    }
}
