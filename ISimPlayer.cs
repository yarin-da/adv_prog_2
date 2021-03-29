using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Adv_Prog_2
{
    // simulation player
    internal interface ISimPlayer : INotifyPropertyChanged
    {
        public int FrameCount { get; set; }
        public string TimerString { get; set; }
        public string SpeedString { get; set; }
        public float Speed { get; set; }
        public int Frame { get; set; }
        public void Play();
        public void Pause();
        public void Stop();
        public void SetFlightDataFile(string filePath);
    }
}
