using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.Entities;

namespace Server.ServiceContracts
{
    [ServiceContract(Name = "INoteSaverService", Namespace = "Server/")]
    interface INoteSaverService
    {
        [OperationContract]
        void SaveNote(User user, Note note);
        [OperationContract]
        void RemoveNote(User user, Note note);
        [OperationContract]
        void EditNote(User user, Note note);
    }
}
