using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.Relax
{
    interface IMusicPlayer
    {
        StreamingPlaybackState PlaybackState { get; }
        void Play(Stream stream);
        void Pause();
        void Stop();
    }
}
