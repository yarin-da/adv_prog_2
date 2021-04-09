﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Adv_Prog_2
{
    class FlightSimPlayer : ISimPlayer
    {
        // constants
        public const int SECOND_MS = 1000;
        public const int DEFAULT_FPS = 10;
        public const int DEFAULT_SLEEP_MS = 100;
        public const float DEFAULT_SPEED = 1.0f;
        public const float MAX_SPEED = 10.0f;
        public const float MIN_SPEED = 0.0f;
        public const int FRAME_SKIP = 50;
        public const float SPEED_SKIP = 0.1f;

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FlightSimPlayer(INetClient netClient, IFileIterator fileIterator)
        {
            this.netClient = netClient;
            this.fileIterator = fileIterator;
            fileIterator.OnLineChanged += SendCurrentFrame;
            fileIterator.OnLineChanged += UpdateTimerString;
            fileIterator.OnLineChanged += Sleep;
        }

        #region Properties

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

        public string MaxTimerString { get; private set; } = "00:00:00";
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
                // update speed, speedString and notify
                float parsedSpeed;
                if (float.TryParse(value, out parsedSpeed))
                {
                    Trace.WriteLine(parsedSpeed);
                    // clamp values
                    if (parsedSpeed > MAX_SPEED)
                    {
                        parsedSpeed = MAX_SPEED;
                    }
                    else if (parsedSpeed < MIN_SPEED)
                    {
                        parsedSpeed = MIN_SPEED;
                    }
                    speedString = String.Format("{0:0.0}", parsedSpeed);
                    Speed = parsedSpeed;
                    NotifyPropertyChanged("SpeedString");
                }
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

        #endregion

        #region SimulationLoop

        private int calcSecondsPassed(int frame)
        {
            // divide by the default amount of frames sent per second
            // i.e. assuming default speed (1.0f)
            // add DEFAULT_FPS - 1 to round up
            return (frame + DEFAULT_FPS - 1) / DEFAULT_FPS;
        }

        private string SecondsToTimer(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours, t.Minutes, t.Seconds);
        }

        public void UpdateTimerString()
        {
            if (speed > 0)
            {
                // get the seconds passed
                int secondsPassed = calcSecondsPassed(Frame);
                TimerString = SecondsToTimer(secondsPassed);
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
                    Thread.Sleep(DEFAULT_SLEEP_MS);
                }
            }

            // there are no frames left so we make sure to set isPlaying to false
            if (!fileIterator.HasNext) {
                IsPlaying = false; 
            }
        }

        #endregion

        #region media_controls

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

        public void FastForward()
        {
            Speed += SPEED_SKIP;
        }

        public void FastBackwards()
        {
            Speed -= SPEED_SKIP;
        }

        public void FrameForward()
        {
            if (Frame + FRAME_SKIP > FrameCount)
            {
                Frame = FrameCount - 1;
            }
            else {
                Frame += FRAME_SKIP;
            }
        }

        public void FrameBackwards()
        {
            if (Frame - FRAME_SKIP < 0)
            {
                Frame = 0;
            }
            else
            {
                Frame -= FRAME_SKIP;
            }
        }

        #endregion

        public void LoadFile(string filePath)
        {
            fileIterator.LoadFile(filePath);
            // do not auto play the simulation as we read a new file
            IsPlaying = false;
            // set the playback to the beginning of the simulation
            Frame = 0;
            FrameCount = fileIterator.LineCount;
            // calculate the maximum simulation timer
            int maxSeconds = calcSecondsPassed(FrameCount);
            MaxTimerString = SecondsToTimer(maxSeconds);
            NotifyPropertyChanged("MaxTimerString");
        }
    }
}
