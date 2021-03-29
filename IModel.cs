using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Adv_Prog_2
{
    interface IModel : INotifyPropertyChanged
    {
        public void Start();
        public bool IsConnected { get; }
        public void Connect(int port, string server);
        public void Disconnect();
        public void Send(string data);
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
