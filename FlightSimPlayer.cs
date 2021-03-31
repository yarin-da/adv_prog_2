using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace Adv_Prog_2
{
    class FlightSimPlayer : ISimPlayer
    {
        // constants
        public const int SECOND_MS = 1000;
        public const int DEFAULT_FPS = 10;
        public const float DEFAULT_SPEED = 1.0f;
        public const float MAX_SPEED = 10.0f;
        public const float MIN_SPEED = 0.1f;

        private readonly object TimerLocker = new object();

        private string timerString = "00:00:00";
        private float speed = DEFAULT_SPEED;
        private string speedString = "1";
        private bool isPlaying = false;
        private int frameCount = 0;
        private INetClient netClient;
        private IFileIterator fileIterator;

        public event PropertyChangedEventHandler PropertyChanged;
        public FlightSimPlayer(INetClient netClient, IFileIterator fileIterator)
        {
            this.netClient = netClient;
            this.fileIterator = fileIterator;
        }

        public bool IsPlaying
        {
            get { return isPlaying; }
            set { isPlaying = value; }
        }

        public int FrameCount
        {
            get { return frameCount; }
            set {
                frameCount = value;
                NotifyPropertyChanged("FrameCount");
            }
        }

        public string TimerString {
            get { 
                lock (TimerLocker)
                {
                    return timerString;
                }
            }
            set {
                lock (TimerLocker)
                {
                    timerString = value;
                    NotifyPropertyChanged("TimerString");
                }
            }
        }

        public string SpeedString
        {
            get { return speedString; }
            set
            {
                try
                {
                    float f = float.Parse(value);
                    speedString = value;
                    speed = f;
                    NotifyPropertyChanged("SpeedString");
                } catch (Exception ex) { Console.WriteLine(ex); }
            }
        }

        public float Speed {
            get { return speed; }
            set {
                if (value >= MIN_SPEED && value <= MAX_SPEED)
                {
                    speed = value;
                    speedString = String.Format("{0:0.0}", speed);
                    NotifyPropertyChanged("SpeedString");
                }
            }
        }
        public int Frame { 
            get {
                return fileIterator.LineNumber;
            } 
            set {
                fileIterator.LineNumber = value;
                NotifyPropertyChanged("Frame");
            }
        }

        public void UpdateTimerString()
        {
            // get the seconds passed
            // divide by the default amount of frames sent per second
            // i.e. assuming default speed (1.0f)
            // add DEFAULT_FPS - 1 to round up 
            int secondsPassed = (Frame + DEFAULT_FPS - 1) / DEFAULT_FPS;
            TimeSpan t = TimeSpan.FromSeconds(secondsPassed);
            TimerString = string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours, t.Minutes, t.Seconds);
        }

        public void LoadFile(string filePath)
        {
            fileIterator.LoadFile(filePath);
            // do not auto play the simulation as we read a new file
            IsPlaying = false;
            // set the playback to the beginning of the simulation
            Frame = 0;
            FrameCount = fileIterator.LineCount;
        }

        public void Stop()
        {
            Pause();
            // return to the beginning of the simulation
            Frame = 0;
        }
        public void Pause()
        {
            IsPlaying = false;
        }

        private void SimulationLoop()
        {
            while (IsPlaying)
            {
                if (Frame >= FrameCount)
                {
                    // exit the simulation loop if there are no frames left
                    IsPlaying = false;
                }
                else
                {
                    try
                    {
                        UpdateTimerString();
                        netClient.Send(fileIterator.CurrentLine);
                        Sleep();
                    }
                    catch (Exception ex)
                    {
                        IsPlaying = false;
                        netClient.Disconnect();
                        Console.WriteLine(ex);
                    }
                    // move to the next frame
                    Frame++;
                }
            }
        }

        public void Sleep()
        {
            // sleep a variable amount of ms (according to the value of speed)
            int ms = (int)(SECOND_MS / (DEFAULT_FPS * speed));
            Thread.Sleep(ms);
        }

        public void Play()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
                // send the frames in a different thread
                // otherwise the GUI is unresponsive
                new Thread(SimulationLoop).Start();
            }
        }
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
