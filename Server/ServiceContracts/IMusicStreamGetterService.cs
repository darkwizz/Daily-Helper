using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server.ServiceContracts
{
    [ServiceContract(Name = "IMusicStreamGetterService", Namespace = "Server/")]
    interface IMusicStreamGetterService
    {
        [OperationContract]
        Stream GetMusicStream();
    }
}
