using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Adv_Prog_2
{
    internal class FlightViewModel : IViewModel
    {
        private IModel model;

        public FlightViewModel()
        {
            INetClient netClient = new FlightNetClient();
            ISimPlayer simPlayer = new FlightSimPlayer(netClient);
            model = new FlightModel(netClient, simPlayer);
            model.PropertyChanged +=
                delegate(Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }
        public int VM_Frame {
            get { return model.Frame; }
            set { model.Frame = value; }
        }
        public int VM_FrameCount { 
            get { return model.FrameCount; }
            set { model.FrameCount = value; }
        }
        public string VM_TimerString {
            get { return model.TimerString; }
            set { model.TimerString = value; }
        }
        public string VM_SpeedString
        {
            get { return model.SpeedString; }
            set { model.SpeedString = value; }
        }
        public float VM_Speed { 
            get { return model.Speed; }
            set { model.Speed = value; }
        }
        public bool VM_Connected { 
            get { return model.IsConnected; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Start()
        {
            model.Start();
        }

        public void Connect(int port, string server)
        {
            model.Connect(port, server);
        }

        public void Disconnect()
        {
            model.Disconnect();
        }

        public void Pause()
        {
            model.Pause();
        }

        public void Stop()
        {
            model.Stop();
        }

        public void SetFlightData(string filePath)
        {
            model.SetFlightDataFile(filePath);
        }
    }
}
