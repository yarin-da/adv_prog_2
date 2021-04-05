using System;
using System.ComponentModel;
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
        public const float MIN_SPEED = 0.0f;

        // mutex
        private readonly object TimerLocker = new object();

        // fields for properties
        private string timerString = "00:00:00";
        private float speed = DEFAULT_SPEED;
        private string speedString = "1";
        private bool isPlaying = false;
        private int frameCount = 0;

        // used to send the server the frames
        private INetClient netClient;
        // used to read flight data
        private IFileIterator fileIterator;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public FlightSimPlayer(INetClient netClient, IFileIterator fileIterator)
        {
            this.netClient = netClient;
            this.fileIterator = fileIterator;
            fileIterator.OnLineChanged += SendCurrentFrame;
            fileIterator.OnLineChanged += UpdateTimerString;
            fileIterator.OnLineChanged += Sleep;
        }

        public bool IsPlaying
        {
            get { return isPlaying; }
            set { isPlaying = value; }
        }

        public int FrameCount
        {
            get { return frameCount; }
            set 
            {
                frameCount = value;
                NotifyPropertyChanged("FrameCount");
            }
        }

        public string TimerString 
        {
            get 
            { 
                lock (TimerLocker)
                {
                    return timerString;
                }
            }
            set 
            {
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
                    // update speed, speedString and notify
                    float f = float.Parse(value);
                    speed = f;
                    speedString = value;
                    NotifyPropertyChanged("SpeedString");
                } catch (Exception ex) { Console.WriteLine(ex); }
            }
        }

        public float Speed 
        {
            get { return speed; }
            set 
            {
                if (value >= MIN_SPEED && value <= MAX_SPEED)
                {
                    // update speed, speedString and notify
                    speed = value;
                    speedString = String.Format("{0:0.0}", speed);
                    NotifyPropertyChanged("SpeedString");
                }
            }
        }
        public int Frame {
            get 
            {
                return fileIterator.LineNumber;
            }
            set 
            {
                fileIterator.LineNumber = value;
                NotifyPropertyChanged("Frame");
            }
        }

        public void UpdateTimerString()
        {
            if (speed > 0)
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

        private void SendCurrentFrame()
        {
            if (IsPlaying)
            {
                netClient.Send(fileIterator.CurrentLine);
            }
        }

        private void SimulationLoop()
        {
            while (IsPlaying && fileIterator.HasNext)
            {
                if (speed > 0)
                {
                    // move to the next frame
                    Frame++;
                }
                else
                {
                    // if speed == 0 then we want to simply sleep and not update the server
                    Thread.Sleep(100);
                }
            }

            // there are no frames left so we make sure to set isPlaying to false
            if (!fileIterator.HasNext) {
                IsPlaying = false; 
            }
        }

        public void Sleep()
        {
            if (IsPlaying && speed > 0)
            {
                // sleep a variable amount of ms (according to the value of speed)
                int ms = (int)(SECOND_MS / (DEFAULT_FPS * speed));
                Thread.Sleep(ms);
            }
        }

        public void Play()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
                // send the frames in a different thread
                // otherwise the GUI is unresponsive
                Thread simulationThread = new Thread(SimulationLoop);
                simulationThread.IsBackground = true;
                simulationThread.Start();
            }
        }
    }
}
