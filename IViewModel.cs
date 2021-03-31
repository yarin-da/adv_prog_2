using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Adv_Prog_2
{
    interface IViewModel : INotifyPropertyChanged
    {
        public int VM_FrameCount { get; set; }
        public int VM_Frame { get; set; }
        public string VM_TimerString { get; set; }
        public string VM_SpeedString { get; set; }
        public float VM_Speed { get; set; }
        public bool VM_Connected { get; }

        public void Start();

        public void Connect(int port, string server);

        public void Disconnect();

        public void Pause();

        public void Stop();

        public void SetFlightData(string filePath);

        public void SetColumnData(string filePath);
    }
}
