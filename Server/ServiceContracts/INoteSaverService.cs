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
    [ServiceContract(Name = "INoteSaverService", Namespace = "Server/")]
    interface INoteSaverService
    {
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void SaveNote(User user, Note note);
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void RemoveNote(Note note);
        [OperationContract]
        [FaultContract(typeof(DatabaseConnectionFault))]
        void EditNote(Note note);
    }
}
