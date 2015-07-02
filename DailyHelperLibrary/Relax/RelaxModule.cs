using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyHelperLibrary.Proxies;
using DailyHelperLibrary.ServiceContracts;

namespace DailyHelperLibrary.Relax
{
    public class RelaxModule
    {
        private IMusicStreamGetterService _musicGetter = new MusicStreamGetter();
        private IMusicPlayer _musicPlayer = new StreamingMusicPlayer();

        public EventResult OnRelaxChosen()
        {
            if (_musicPlayer.PlaybackState != StreamingPlaybackState.Stopped)
            {
                return new EventResult(false, "Song already is played");
            }
            try
            {
                Stream musicStream = _musicGetter.GetMusicStream();
                _musicPlayer.Play(musicStream);
                return new EventResult(true);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Some fault on server: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }

        public EventResult OnNextChosen()
        {
            if (_musicPlayer.PlaybackState != StreamingPlaybackState.Stopped)
            {
                _musicPlayer.Stop();
                // There is a temporary crutch
                // because sample recording thread can't catch to close main loop
                // before _musicPlayer creates new stream
                // now it is used for syncronization purposes
                
                // Thread.Sleep(750);
            }
            return OnRelaxChosen();
        }

        public EventResult OnStopChosen()
        {
            _musicPlayer.Stop();
            return new EventResult(true);
        }
    }
}
