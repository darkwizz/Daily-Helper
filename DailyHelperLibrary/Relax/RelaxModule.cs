using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceContracts;

namespace DailyHelperLibrary.Relax
{
    public class RelaxModule
    {
        private IMusicStreamGetterService _musicGetter = new MusicStreamGetterStub();
        private IMusicPlayer _musicPlayer = new StreamingMusicPlayer();

        public EventResult OnRelaxChosen()
        {
            Stream musicStream = _musicGetter.GetMusicStream();
            _musicPlayer.Play(musicStream);
            return new EventResult(true);
        }

        public EventResult OnNextChosen()
        {
            return OnRelaxChosen();
        }

        public EventResult OnStopChosen()
        {
            _musicPlayer.Stop();
            return new EventResult(true);
        }
    }
}
