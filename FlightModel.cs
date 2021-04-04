using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using OxyPlot;

namespace Adv_Prog_2
{
    class FlightModel : IModel
    {
        private INetClient netClient;
        private ISimPlayer simPlayer;
        private IFileIterator fileIterator;
        private IGraphPlotter selectedPlot;
        private IGraphPlotter correlatedPlot;
        private XMLParser dataParser;
        private string selectedColumn = "";

        public FlightModel()
        {
            this.netClient = new FlightNetClient();
            this.fileIterator = new FlightDataIterator();
            this.simPlayer = new FlightSimPlayer(netClient, fileIterator);
            this.dataParser = new XMLParser();
            this.selectedPlot = new FlightGraphPlotter();
            this.correlatedPlot = new FlightGraphPlotter();
            this.simPlayer.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged(e.PropertyName);
                };
            this.selectedPlot.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("SelectedPlot");
                };
            this.correlatedPlot.PropertyChanged +=
               delegate (Object sender, PropertyChangedEventArgs e)
               {
                   NotifyPropertyChanged("CorrelatedPlot");
               };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public List<string> ColumnList { 
            get { 
                if (dataParser.HasData)
                {
                    return dataParser.getTitles();
                }
                return null;
            } 
        }
        public PlotModel SelectedPlot { get { return selectedPlot.Plot; } }
        public PlotModel CorrelatedPlot { get { return correlatedPlot.Plot; } }
        public string SelectedColumn { 
            get {
                return selectedColumn; }
            set {
                // call the delegate function every few milliseconds
                selectedPlot.setDataCallback(
                    delegate (object sender, System.Timers.ElapsedEventArgs e)
                    {
                        int index = dataParser.GetColumnNumber(value);
                        if (index >= 0)
                        {
                            selectedColumn = value;
                            // get the values to display on our graph
                            float[] values = fileIterator.GetAdjacentValues(index);
                            if (values != null)
                            {
                                // plot the values (update the graph)
                                selectedPlot.LoadData(values);
                            }
                        }
                    }
                );
                correlatedPlot.setDataCallback(
                    delegate (object sender, System.Timers.ElapsedEventArgs e)
                    {
                        int index = dataParser.GetColumnNumber("altitude-ft");
                        if (index >= 0)
                        {
                            selectedColumn = value;
                            // get the values to display on our graph
                            float[] values = fileIterator.GetAdjacentValues(index);
                            if (values != null)
                            {
                                // plot the values (update the graph)
                                correlatedPlot.LoadData(values);
                            }
                        }
                    }
                );
            } 
        }
        public bool IsConnected { get { return netClient.IsConnected; } }
        public void Connect(int port, string server) { netClient.Connect(port, server); }
        public void Disconnect() {
            // pause (i.e. stop sending the server messages)
            // because otherwise the socket might throw an exception (invalid write)
            Pause();
            netClient.Disconnect();
        }
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
        public void Play() { 
            if (netClient.IsConnected)
            {
                selectedPlot.StartCallbackTimer();
                // start play the graph of correlated values
                correlatedPlot.StartCallbackTimer();
                simPlayer.Play();
            }
        }
        public void Pause() {
            // stop updating the graph when paused
            selectedPlot.StopCallbackTimer();
            // stop updating the graph of correlated when paused
            correlatedPlot.StopCallbackTimer();
            simPlayer.Pause(); 
        }
        public void Stop() {
            // stop updating the graph when stopped
            selectedPlot.StopCallbackTimer();
            // stop updating the graph of correlated when paused
            correlatedPlot.StopCallbackTimer();
            simPlayer.Stop();
        }
        public void SetFlightDataFile(string filePath) {
            simPlayer.LoadFile(filePath);
        }
        public void SetColumnData(string filePath) {
            // update the columnList and notify the view
            dataParser.LoadFile(filePath);
            NotifyPropertyChanged("ColumnList");
        }
    }
}
