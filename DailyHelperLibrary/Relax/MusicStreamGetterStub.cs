using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceContracts;

namespace DailyHelperLibrary.Relax
{
    class MusicStreamGetterStub: IMusicStreamGetterService
    {
        public Stream GetMusicStream()
        {
            FileStream stream = new FileStream(@"D:\Folder\Music\Patience\Bruce Dickinson – Gates Of Urizen.mp3", FileMode.Open);
            return stream;
        }
    }
}
