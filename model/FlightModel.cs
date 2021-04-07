using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using Adv_Prog_2.model.joystick;
using Adv_Prog_2.model.joystick_and_dashboard;
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

        private IJoystick joystick;
        private IDashboard dashboard;

        private string selectedColumn = "";

        #region ctor

        private static IModel instance = null;
        public static IModel GetInstance()
        {
            if (instance == null)
            {
                instance = new FlightModel();
            }
            return instance;
        }

        private FlightModel()
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

            joystick = new FlightJoystick(fileIterator, dataAnalyzer);
            joystick.PropertyChanged += PropertyChangedFunction;
            dashboard = new FlightDashboard(fileIterator, dataAnalyzer);
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
        public float AltimeterMax { get { return dashboard.AltimeterMax; } }
        public float AirspeedMax { get { return dashboard.AirspeedMax; } }
        public float DirectionMax { get { return dashboard.DirectionMax; } }
        public float YawMax { get { return dashboard.YawMax; } }
        public float PitchMax { get { return dashboard.PitchMax; } }
        public float RollMax { get { return dashboard.RollMax; } }
        public float AltimeterMin { get { return dashboard.AltimeterMin; } }
        public float AirspeedMin { get { return dashboard.AirspeedMin; } }
        public float DirectionMin { get { return dashboard.DirectionMin; } }
        public float YawMin { get { return dashboard.YawMin; } }
        public float PitchMin { get { return dashboard.PitchMin; } }
        public float RollMin { get { return dashboard.RollMin; } }
        #endregion

        #region joystick
        public float MaxThrottle { get { return joystick.MaxThrottle; } }
        public float MinThrottle { get { return joystick.MinThrottle; } }
        public float Throttle1 { get { return joystick.Throttle1; } }
        public float Throttle2 { get { return joystick.Throttle2; } }
        public float MaxRudder { get { return joystick.MaxRudder; } }
        public float MinRudder { get { return joystick.MinRudder; } }
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

        #endregion

        #region network_client

        public static SolidColorBrush green = new SolidColorBrush(Colors.LightGreen);
        public static SolidColorBrush red = new SolidColorBrush(Colors.Red);
        public const string notConnectedString = "Not Connected";
        public const string connectionFailedString = "Connection Failed!";
        public const string connectedString = "Connection Established";
        public const string fileMissingString = "Missing File";
        public const string connectButtonConnect = "Connect";
        public const string connectButtonDisconnect = "Disconnect";

        public string ConnectButtonText { get; private set; } = connectButtonConnect;
        public string ConnectStatus { get; private set; } = notConnectedString;
        public SolidColorBrush ConnectStatusColor { get; private set; } = red;
        public string FlightDataFileName { get; private set; } = fileMissingString;
        public SolidColorBrush FlightDataFileNameColor { get; private set; } = red;
        public string MetaDataFileName { get; private set; } = fileMissingString;
        public SolidColorBrush MetaDataFileNameColor { get; private set; } = red;
        public string ServerPort { get; set; } = "5400";
        public string ServerIP { get; set; } = "127.0.0.1";
        public bool IsConnected { get { return netClient.IsConnected; } }
        public void ToggleConnection()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            else {
                Connect();
            }
        }
        public void Connect() {
            bool connectSuccess = false;
            int port;
            if (int.TryParse(ServerPort, out port))
            {
                try
                {
                    netClient.Connect(port, ServerIP);
                    connectSuccess = true;
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }

            ConnectStatus = connectSuccess ? connectedString : connectionFailedString;
            ConnectStatusColor = connectSuccess ? green : red;
            ConnectButtonText = connectSuccess ? connectButtonDisconnect : connectButtonConnect;
            NotifyPropertyChanged("ConnectStatus");
            NotifyPropertyChanged("ConnectStatusColor");
            NotifyPropertyChanged("ConnectButtonText");
        }
        public void Disconnect() 
        {
            // pause (i.e. stop sending the server messages)
            // because otherwise the socket might throw an exception (invalid write)
            Pause();
            netClient.Disconnect();
            ConnectStatus = notConnectedString;
            ConnectStatusColor = red;
            ConnectButtonText = connectButtonConnect;
            NotifyPropertyChanged("ConnectStatus");
            NotifyPropertyChanged("ConnectStatusColor");
            NotifyPropertyChanged("ConnectButtonText");
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

        public void FastForward()
        {
            simPlayer.FastForward();
        }

        public void FastBackwards()
        {
            simPlayer.FastBackwards();
        }

        public void FrameForward()
        {
            simPlayer.FrameForward();
        }

        public void FrameBackwards()
        {
            simPlayer.FrameBackwards();
        }

        #endregion

        #region data_files
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
        #endregion

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
