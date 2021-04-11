using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Adv_Prog_2.model.graph;
using Adv_Prog_2.model.joystick;
using Adv_Prog_2.model.dashboard;
using OxyPlot;
using Adv_Prog_2.model.data;
using Adv_Prog_2.model.net;
using Adv_Prog_2.model.mediacontrol;

namespace Adv_Prog_2.model
{
    class FlightModel : IModel
    {
        private DataAnalyzer dataAnalyzer;
        private AnomalyAnalyzer anomalyAnalyzer;
        private LibraryLoader libLoader;
        private XMLParser dataParser;

        private BaseGraph selectedPlot;
        private BaseGraph correlatedPlot;
        private BaseGraph anomalyPlot;

        private INetClient netClient;
        private IFileIterator fileIterator;
        private ISimPlayer simPlayer;
        private IJoystick joystick;
        private IDashboard dashboard;

        private string selectedColumn = "";

        #region ctor
        // The model operates as a singleton
        // This is because every ViewModel needs to communicate 
        // with the same model object
        private static IModel instance = null;
        public static IModel GetInstance()
        {
            if (instance == null)
            {
                instance = new FlightModel();
            }
            return instance;
        }

        // Initialize Objects and add notify events
        private FlightModel()
        {
            netClient = new FlightNetClient();
            fileIterator = new FlightDataIterator();
            dataParser = new XMLParser();
            dataAnalyzer = new DataAnalyzer();
            libLoader = new LibraryLoader();
            anomalyAnalyzer = new AnomalyAnalyzer(libLoader);
            selectedPlot = new MonitorGraph(dataAnalyzer, fileIterator);
            correlatedPlot = new MonitorGraph(dataAnalyzer, fileIterator);
            anomalyPlot = new AnomalyGraph(dataAnalyzer, fileIterator, anomalyAnalyzer);
            simPlayer = new FlightSimPlayer(netClient, fileIterator);
            joystick = new FlightJoystick(fileIterator, dataAnalyzer);
            dashboard = new FlightDashboard(fileIterator, dataAnalyzer);
            
            // add notify events
            simPlayer.PropertyChanged += PropertyChangedFunction;
            joystick.PropertyChanged += PropertyChangedFunction;
            dashboard.PropertyChanged += PropertyChangedFunction;
        }
        #endregion

        #region property_changed

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PropertyChangedFunction(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        #endregion

        #region dashboard
        public float Altimeter { get { return dashboard.Altimeter; } }
        public float Airspeed { get { return dashboard.Airspeed; } }
        public float Direction { get { return dashboard.Direction; } }
        public float Yaw { get { return dashboard.Yaw; } }
        public float Pitch { get { return dashboard.Pitch; } }
        public float Roll { get { return dashboard.Roll; } }
        #endregion

        #region joystick
        public float Throttle1 { get { return joystick.Throttle1; } }
        public float Throttle2 { get { return joystick.Throttle2; } }
        public float Rudder { get { return joystick.Rudder; } }
        public float KnobX { get { return joystick.KnobX; } }
        public float KnobY { get { return joystick.KnobY; } }
        #endregion

        #region graphs
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
                if (selectedColumn == value) { return; }

                selectedColumn = value;
                // make sure selectedPlot updates automatically every few milliseconds
                selectedPlot.SetDataCallback(selectedColumn);
                string correlatedColumn = dataAnalyzer.GetCorrelatedColumn(value);
                if (correlatedColumn != null)
                {
                    // make sure anomalyPlot updates automatically every few milliseconds
                    correlatedPlot.SetDataCallback(correlatedColumn);
                }

                anomalyPlot.SetDataCallback(selectedColumn);
            } 
        }
        #endregion

        #region network_client
        public string ServerPort { get; set; } = "5400";
        public string ServerIP { get; set; } = "127.0.0.1";
        public bool IsConnected { get { return netClient.IsConnected; } }
        
        public bool Connect() {
            int port;
            // TryParse incase the user entered non-float string
            if (int.TryParse(ServerPort, out port))
            {
                try
                {
                    netClient.Connect(port, ServerIP);
                    // won't get here if an exception was thrown during connect 
                    return true;
                }
                catch (Exception ex) { Trace.WriteLine(ex); }
            }
            return false;
        }
        public void Disconnect() 
        {
            // pause (i.e. stop sending the server messages)
            // because otherwise the socket might throw an exception (invalid write)
            Pause();
            netClient.Disconnect();
        }
        #endregion

        #region media_controls_and_properties
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
        public string MaxTimerString { 
            get { return simPlayer.MaxTimerString; }
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
                // start playing the simulation
                SendMediaControl("Play");
            }
        }
        public void Pause() 
        {
            // stop updating the graphs when paused
            selectedPlot.StopCallbackTimer();
            correlatedPlot.StopCallbackTimer();
            anomalyPlot.StopCallbackTimer();
            // stop playing the simulation
            SendMediaControl("Pause");
        }
        public void Stop() 
        {
            // stop updating the graphs when stopped
            selectedPlot.StopCallbackTimer();
            correlatedPlot.StopCallbackTimer();
            anomalyPlot.StopCallbackTimer();
            // stop playing the simulation
            SendMediaControl("Stop");
        }

        public void SendMediaControl(string control)
        {
            simPlayer.SendControl(control);
        }

        #endregion

        #region data_files
        public void SetLearningData(string filePath) 
        {
            anomalyAnalyzer.SetLearningData(filePath);
        }

        public void SetMetaData(string filePath) 
        {
            // update the columnList
            dataParser.LoadFile(filePath);
            // parse the XML file to get the column titles
            var titles = dataParser.GetTitles();
            dataAnalyzer.SetMetaData(titles);
            anomalyAnalyzer.SetMetaData(dataAnalyzer.StringListToArr(titles));
            // notify the view
            NotifyPropertyChanged("ColumnList");
        }

        public void SetAnomalyData(string filePath)
        {
            simPlayer.LoadFile(filePath);
            dataAnalyzer.SetFlightData(filePath);
            anomalyAnalyzer.SetFlightData(filePath);
        }

        public void SetAlgorithmData(string filePath)
        {
            // Load the DLL file
            libLoader.LoadLibrary(filePath);
        }
        #endregion

        // close all resources when the user exits the application 
        public void CloseApplication()
        {
            // stop updating the graphs 
            selectedPlot.StopCallbackTimer();
            correlatedPlot.StopCallbackTimer();
            anomalyPlot.StopCallbackTimer();
            // stop sending frames to the server
            SendMediaControl("Stop");
            // destroy data resources
            dataAnalyzer.DestroyResources();
            anomalyAnalyzer.DestroyResources();
            // close the socket
            netClient.Disconnect();
        }
    }
}
