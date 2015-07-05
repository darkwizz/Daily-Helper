using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.ServiceContracts
{
    [ServiceContract(Name = "ISocialNetworkAccountInfoSaverService", Namespace = "Server/")]
    interface ISocialNetworkAccountInfoSaverService
    {
        [OperationContract]
        void UpdateAccountInfo(ServiceUser user, ServiceSocialNetworkAccountInfo info);
        [OperationContract]
        Dictionary<Guid, ServiceSocialNetworkAccountInfo> GetAccounts(ServiceUser user);
    }
}
