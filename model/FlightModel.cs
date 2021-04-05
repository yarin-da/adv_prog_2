using System;
using System.Collections.Generic;
using System.ComponentModel;
using Adv_Prog_2.model.joystick;
using OxyPlot;

namespace Adv_Prog_2
{
    class FlightModel : IModel
    {
        private INetClient netClient;
        private IFileIterator fileIterator;
        private DataAnalyzer dataAnalyzer;
        private XMLParser dataParser;

        private ISimPlayer simPlayer;

        private IGraphPlotter selectedPlot;
        private IGraphPlotter correlatedPlot;
        private IGraphPlotter anomalyPlot;

        private string selectedColumn = "";

        public FlightModel()
        {
            netClient = new FlightNetClient();
            fileIterator = new FlightDataIterator();
            dataParser = new XMLParser();
            dataAnalyzer = new DataAnalyzer();

            selectedPlot = new FlightGraphPlotter("SelectedPlot");
            selectedPlot.PropertyChanged += PropertyChangedFunction;
            correlatedPlot = new FlightGraphPlotter("CorrelatedPlot");
            correlatedPlot.PropertyChanged += PropertyChangedFunction;
            anomalyPlot = new FlightGraphPlotter("AnomalyPlot");
            anomalyPlot.PropertyChanged += PropertyChangedFunction;

            simPlayer = new FlightSimPlayer(netClient, fileIterator);
            simPlayer.PropertyChanged += PropertyChangedFunction;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void PropertyChangedFunction(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        public List<string> ColumnList 
        { 
            get 
            { 
                if (dataParser.HasData)
                {
                    return dataParser.GetTitles();
                }
                return null;
            } 
        }
        public PlotModel SelectedPlot { get { return selectedPlot.Plot; } }
        public PlotModel CorrelatedPlot { get { return correlatedPlot.Plot; } }
        public PlotModel AnomalyPlot { get { return anomalyPlot.Plot; } }
        public string SelectedColumn 
        { 
            get { return selectedColumn; }
            set 
            {
                selectedColumn = value;
                // make sure selectedPlot updates automatically every few milliseconds
                selectedPlot.SetDataCallback(selectedColumn, fileIterator, dataAnalyzer);
                string correlatedColumn = dataAnalyzer.GetCorrelatedColumn(value);
                if (correlatedColumn != null)
                {
                    // make sure anomalyPlot updates automatically every few milliseconds
                    correlatedPlot.SetDataCallback(correlatedColumn, fileIterator, dataAnalyzer);
                }

                float a, b;
                dataAnalyzer.GetLinearReg(selectedColumn, out a, out b);
                anomalyPlot.LoadData(selectedColumn, a, b);
            } 
        }

        public bool IsConnected { get { return netClient.IsConnected; } }
        public void Connect(int port, string server) { netClient.Connect(port, server); }
        public void Disconnect() 
        {
            // pause (i.e. stop sending the server messages)
            // because otherwise the socket might throw an exception (invalid write)
            Pause();
            netClient.Disconnect();
        }

        public int FrameCount 
        { 
            get { return simPlayer.FrameCount; } 
            set { simPlayer.FrameCount = value; }
        }
        public string TimerString 
        {
            get { return simPlayer.TimerString; }
            set { simPlayer.TimerString = value; }
        }
        public string SpeedString 
        {
            get { return simPlayer.SpeedString; }
            set { simPlayer.SpeedString = value; }
        }
        public float Speed 
        {
            get { return simPlayer.Speed; }
            set { simPlayer.Speed = value; }
        }
        public int Frame 
        {
            get { return simPlayer.Frame; }
            set { simPlayer.Frame = value; }
        }
        public void Play() 
        { 
            if (netClient.IsConnected)
            {
                // start updating the graphs
                selectedPlot.StartCallbackTimer();
                correlatedPlot.StartCallbackTimer();
                anomalyPlot.StartCallbackTimer();
                simPlayer.Play();
            }
        }
        public void Pause() 
        {
            // stop updating the graphs when paused
            selectedPlot.StopCallbackTimer();
            correlatedPlot.StopCallbackTimer();
            anomalyPlot.StopCallbackTimer();
            simPlayer.Pause(); 
        }
        public void Stop() 
        {
            // stop updating the graphs when stopped
            selectedPlot.StopCallbackTimer();
            correlatedPlot.StopCallbackTimer();
            anomalyPlot.StopCallbackTimer();
            simPlayer.Stop();
        }

        public void SetFlightDataFile(string filePath) 
        {
            simPlayer.LoadFile(filePath);
            dataAnalyzer.SetFlightData(filePath);
        }
        public void SetColumnData(string filePath) 
        {
            // update the columnList and notify the view
            dataParser.LoadFile(filePath);
            dataAnalyzer.SetMetaData(dataParser.GetTitles());
            NotifyPropertyChanged("ColumnList");
        }

        // close all resources when the user exits the application
        public void CloseApplication()
        {
            // stop updating the graphs 
            selectedPlot.StopCallbackTimer();
            correlatedPlot.StopCallbackTimer();
            anomalyPlot.StopCallbackTimer();
            // stop sending frames to the server
            simPlayer.Stop();
            // destroy data resources
            dataAnalyzer.DestroyResources();
            // close the socket
            netClient.Disconnect();
        }
    }
}
