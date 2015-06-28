using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceContracts;

namespace DailyHelperLibrary.Proxies
{
    class MusicStreamGetter : ClientBase<IMusicStreamGetterService>, IMusicStreamGetterService
    {
        public MusicStreamGetter() :
            base("GetMusicStreamEndpoint")
            { }

        public Stream GetMusicStream()
        {
            return Channel.GetMusicStream();
        }
    }
}
