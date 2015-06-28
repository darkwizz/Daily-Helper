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
        void RemoveNote(Note note);
        [OperationContract]
        void EditNote(Note note);
    }
}
