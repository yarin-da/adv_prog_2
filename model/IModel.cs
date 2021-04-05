using System.Collections.Generic;
using System.ComponentModel;
using OxyPlot;

namespace Adv_Prog_2
{
    interface IModel : INotifyPropertyChanged
    {
        public List<string> ColumnList { get; }
        public PlotModel SelectedPlot { get; }
        public PlotModel CorrelatedPlot { get; }
        public PlotModel AnomalyPlot { get; }
        public string SelectedColumn { get; set; }
        public bool IsConnected { get; }
        public void Connect(int port, string server);
        public void Disconnect();
        public int FrameCount { get; set; }
        public string TimerString { get; set; }
        public string SpeedString { get; set; }
        public float Speed { get; set; }
        public int Frame { get; set; }
        public void Play();
        public void Pause();
        public void Stop();
        public void SetFlightDataFile(string filePath);
        public void SetColumnData(string filePath);
        public void CloseApplication();
    }
}
