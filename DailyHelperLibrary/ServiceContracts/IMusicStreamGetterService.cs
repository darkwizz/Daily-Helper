using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.ServiceContracts
{
    [ServiceContract(Name = "IMusicStreamGetter", Namespace = "Server/")]
    interface IMusicStreamGetterService
    {
        [OperationContract]
        Stream GetMusicStream();
    }
}
