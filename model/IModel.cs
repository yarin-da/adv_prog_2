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
        #endregion

        #region joystick
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
        public string ServerPort { get; set; }
        public string ServerIP { get; set; }
        public bool IsConnected { get; }
        public bool Connect();
        public void Disconnect();
        #endregion

        #region media_control
        public int FrameCount { get; }
        public string TimerString { get; set; }
        public string MaxTimerString { get; }
        public string SpeedString { get; set; }
        public float Speed { get; }
        public int Frame { get; set; }

        public void Play();
        public void Pause();
        public void Stop();
        public void SendMediaControl(string control);
        #endregion

        #region data_files
        public void SetLearningData(string filePath);
        public void SetAnomalyData(string filePath);
        public void SetMetaData(string filePath);
        public void SetAlgorithmData(string filePath);
        #endregion

        public void CloseApplication();
    }
}
