using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace DailyHelperLibrary.Relax
{
    enum StreamingPlaybackState
    {
        Stopped,
        Playing,
        Buffering,
        Paused
    }

    class StreamingMusicPlayer: IMusicPlayer
    {
        private BufferedWaveProvider _bufferedWaveProvider = null;
        private IWavePlayer _waveOut = null;
        private volatile StreamingPlaybackState _playbackState;
        private volatile bool _fullyDownloaded = false;
        private Stream _currentStream = null;
        private System.Threading.Timer _timer = null;
        private byte[] _buffer = new byte[1024 * 64];
        private ManualResetEvent _manualEvent = new ManualResetEvent(true);
        private bool _isPlayerInitted = false;

        public StreamingPlaybackState PlaybackState
        {
            get
            {
                return _playbackState;
            }
        }
        
        public void Pause()
        {
            _manualEvent.WaitOne();
            _manualEvent.Reset();
            if (_playbackState == StreamingPlaybackState.Playing && _currentStream != null && _currentStream.CanRead)
            {
                _playbackState = StreamingPlaybackState.Buffering;
                _waveOut.Pause();
            }
            _manualEvent.Set();
        }

        public void Stop()
        {
            if (_currentStream != null)
            {
                if (_playbackState != StreamingPlaybackState.Stopped)
                {
                    _playbackState = StreamingPlaybackState.Stopped;
                    if (_waveOut != null)
                    {
                        _waveOut.Stop();
                        _waveOut.Dispose();
                        _waveOut = null;
                    }
                    _timer.Dispose();

                    _bufferedWaveProvider.ClearBuffer();
                    _bufferedWaveProvider = null;

                    // Console.WriteLine("Stop() waits...");
                    _manualEvent.WaitOne();
                    _manualEvent.Reset();
                    // Console.WriteLine("Stop() resets...");

                    _currentStream.Close();
                    _currentStream = null;

                    _manualEvent.Set();
                    // Console.WriteLine("Stop() frees...");
                }
            }
        }

        public void Play(Stream stream)
        {
            if (_playbackState == StreamingPlaybackState.Playing)
            {
                Console.WriteLine("Song is already played");
                return;
            }
            if (stream == null)
            {
                Console.WriteLine("No input stream");
                return;
            }

            _manualEvent.WaitOne();
            _manualEvent.Reset();
            _playbackState = StreamingPlaybackState.Buffering;

            _currentStream = stream;

            ThreadPool.QueueUserWorkItem((state) => AddSamples(), null);
            _timer = new System.Threading.Timer((state) => PlaybackCallback(), null, TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMilliseconds(250));
            _manualEvent.Set();
        }

        private void AddSamples()
        {
            IMp3FrameDecompressor decompressor = null;
            try
            {
                // Console.WriteLine("AddSamples(stream) waits...");
                _manualEvent.WaitOne();
                _manualEvent.Reset();
                // Console.WriteLine("AddSamples(stream) resets...");
                var readFullyStream = new ReadFullyStream(_currentStream);
                do
                {
                    if (IsBufferNearlyFull)
                    {
                        //Console.WriteLine("Buffer getting full, taking a break"); // logging
                        Thread.Sleep(750);
                    }
                    else
                    {
                        Mp3Frame frame;
                        try
                        {
                            frame = Mp3Frame.LoadFromStream(readFullyStream);

                            if (frame == null)
                            {
                                _fullyDownloaded = true;
                                break;
                            }
                            //Console.WriteLine("Bit rate => " + frame.BitRate);
                        }
                        catch (EndOfStreamException ex)
                        {
                            _fullyDownloaded = true;
                            Console.WriteLine("End of stream " + ex);
                            // reached the end of the MP3 file / stream
                            break;
                        }
                        if (decompressor == null)
                        {
                            // don't think these details matter too much - just help ACM select the right codec
                            // however, the buffered provider doesn't know what sample rate it is working at
                            // until we have a frame
                            decompressor = CreateFrameDecompressor(frame);
                            _bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);
                            _bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20); // allow us to get well ahead of ourselves
                            //this.bufferedWaveProvider.BufferedDuration = 250;
                        }
                        int decompressed = decompressor.DecompressFrame(frame, _buffer, 0);
                        //Debug.WriteLine(String.Format("Decompressed a frame {0}", decompressed));
                        _bufferedWaveProvider.AddSamples(_buffer, 0, decompressed);
                    }
                } while (_playbackState != StreamingPlaybackState.Stopped);
                // Console.WriteLine("AddSamples(stream) frees...");
                //Debug.WriteLine("Exiting");
                // was doing this in a finally block, but for some reason
                // we are hanging on response stream .Dispose so never get there
            }
            catch (IOException ex)
            {
                Console.WriteLine("Unavailable stream"); // logging
                if (ex.InnerException is CommunicationException)
                {
                    ErrorNotifier.NotifyUser("Connection with server failed. Check your internet connection. " +
                            "Otherwise, wait, please, while server is being repaired");
                }
                else
                {
                    ErrorNotifier.NotifyUser("Unavailable stream. Error message: " + ex.Message);
                }
                Stop();
            }
            finally
            {
                _manualEvent.Set();
                if (decompressor != null)
                {
                    decompressor.Dispose();
                }
            }
        }

        private IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                frame.FrameLength, frame.BitRate);
            return new AcmMp3FrameDecompressor(waveFormat);
        }

        private bool IsBufferNearlyFull
        {
            get
            {
                return _bufferedWaveProvider != null &&
                       _bufferedWaveProvider.BufferLength - _bufferedWaveProvider.BufferedBytes
                       < _bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4;
            }
        }

        private void PlaybackCallback()
        {
            if (_playbackState != StreamingPlaybackState.Stopped)
            {
                if (_waveOut == null && _bufferedWaveProvider != null)
                {
                    //Console.WriteLine("Creating WaveOut Device");
                    _waveOut = new WaveOutEvent();
                    _isPlayerInitted = false;
                    _waveOut.Init(_bufferedWaveProvider);
                    _isPlayerInitted = true;
                    //Console.WriteLine("After INIT");
                }
                else if (_bufferedWaveProvider != null)
                {
                    var bufferedSeconds = _bufferedWaveProvider.BufferedDuration.TotalSeconds;
                    // make it stutter less if we buffer up a decent amount before playing
                    if (bufferedSeconds < 0.5 && _playbackState == StreamingPlaybackState.Playing && !_fullyDownloaded)
                    {
                        MakePause();
                    }
                    else if (bufferedSeconds > 4 && _playbackState == StreamingPlaybackState.Buffering)
                    {
                        Play();
                    }
                    else if (_fullyDownloaded && bufferedSeconds == 0)
                    {
                        //Debug.WriteLine("Reached end of stream");
                        Stop();
                    }
                }
            }
        }

        private void Play()
        {
            if (!_isPlayerInitted)
            {
                return;
            }
            _waveOut.Play();
            Console.WriteLine("Played...");
            //Debug.WriteLine(String.Format("Started playing, waveOut.PlaybackState={0}", waveOut.PlaybackState));
            _playbackState = StreamingPlaybackState.Playing;
        }

        private void MakePause()
        {
            _playbackState = StreamingPlaybackState.Buffering;
            _waveOut.Pause();
            //Debug.WriteLine(String.Format("Paused to buffer, waveOut.PlaybackState={0}", waveOut.PlaybackState));
        }
    }
}
