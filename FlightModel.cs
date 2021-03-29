using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Adv_Prog_2
{
    class FlightModel : IModel
    {
        private INetClient netClient;
        private ISimPlayer simPlayer;

        public FlightModel(INetClient netClient, ISimPlayer simPlayer)
        {
            this.netClient = netClient;
            this.simPlayer = simPlayer;
            simPlayer.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged(e.PropertyName);
                };
        }

        public void Start()
        {
            simPlayer.Play();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public bool IsConnected { get { return netClient.IsConnected; } }
        public void Connect(int port, string server) { netClient.Connect(port, server); }
        public void Disconnect() { netClient.Disconnect(); }
        public void Send(string data) { netClient.Send(data); }
        public int FrameCount { 
            get { return simPlayer.FrameCount; } 
            set { simPlayer.FrameCount = value; }
        }
        public string TimerString {
            get { return simPlayer.TimerString; }
            set { simPlayer.TimerString = value; }
        }
        public string SpeedString {
            get { return simPlayer.SpeedString; }
            set { simPlayer.SpeedString = value; }
        }
        public float Speed {
            get { return simPlayer.Speed; }
            set { simPlayer.Speed = value; }
        }
        public int Frame {
            get { return simPlayer.Frame; }
            set { simPlayer.Frame = value; }
        }
        public void Play() { simPlayer.Play(); }
        public void Pause() { simPlayer.Pause(); }
        public void Stop() { simPlayer.Stop(); }
        public void SetFlightDataFile(string filePath) { simPlayer.SetFlightDataFile(filePath); }
    }
}
