using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.ServiceContracts
{
    [ServiceContract(Name = "INoteSaverService", Namespace = "Server/")]
    interface INoteSaverService
    {
        [OperationContract]
        void SaveNote(ServiceUser user, ServiceNote note);
        [OperationContract]
        void RemoveNote(ServiceNote note);
        [OperationContract]
        void EditNote(ServiceNote note);
        [OperationContract]
        Dictionary<Guid, ServiceNote> GetNotes(ServiceUser user);
    }
}
