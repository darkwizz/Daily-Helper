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
    [ServiceContract(Name = "ISocialNetworkAccountInfoSaverService", Namespace = "Server/")]
    interface ISocialNetworkAccountInfoSaverService
    {
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void UpdateAccountInfo(User user, SocialNetworkAccountInfo info);
    }
}
