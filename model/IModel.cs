using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using OxyPlot;

namespace Adv_Prog_2
{
    interface IModel : INotifyPropertyChanged
    {
        #region dashboard
        public float Altimeter { get; }
        public float Airspeed { get; }
        public float Direction { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }
        public float AltimeterMax { get; }
        public float AirspeedMax { get; }
        public float DirectionMax { get; }
        public float YawMax { get; }
        public float PitchMax { get; }
        public float RollMax { get; }
        public float AltimeterMin { get; }
        public float AirspeedMin { get; }
        public float DirectionMin { get; }
        public float YawMin { get; }
        public float PitchMin { get; }
        public float RollMin { get; }
        #endregion

        #region joystick
        public float MaxThrottle { get; }
        public float MinThrottle { get; }
        public float MaxRudder { get; }
        public float MinRudder { get; }
        public float Throttle1 { get; }
        public float Throttle2 { get; }
        public float Rudder { get; }
        public float KnobX { get; }
        public float KnobY { get; }
        #endregion

        #region graphs
        public List<string> ColumnList { get; }
        public PlotModel SelectedPlot { get; }
        public PlotModel CorrelatedPlot { get; }
        public PlotModel AnomalyPlot { get; }
        public string SelectedColumn { get; set; }
        #endregion

        #region networking
        public string ConnectButtonText { get; }
        public string ConnectStatus { get; }
        public SolidColorBrush ConnectStatusColor { get; }
        public string FlightDataFileName { get; }
        public SolidColorBrush FlightDataFileNameColor { get; }
        public string MetaDataFileName { get; }
        public SolidColorBrush MetaDataFileNameColor { get; }
        public string ServerPort { get; set; }
        public string ServerIP { get; set; }
        public bool IsConnected { get; }
        public void ToggleConnection();
        #endregion

        #region media_control
        public int FrameCount { get; set; }
        public string TimerString { get; set; }
        public string SpeedString { get; set; }
        public float Speed { get; set; }
        public int Frame { get; set; }

        public void Play();
        public void Pause();
        public void Stop();
        public void FastForward();
        public void FastBackwards();
        public void FrameForward();
        public void FrameBackwards();
        #endregion

        #region data_files
        public void SetFlightDataFile(string filePath);
        public void SetColumnData(string filePath);
        #endregion

        public void CloseApplication();
    }
}
