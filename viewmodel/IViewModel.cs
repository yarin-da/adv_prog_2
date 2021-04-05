using System.Collections.Generic;
using System.ComponentModel;
using OxyPlot;

namespace Adv_Prog_2
{
    interface IViewModel : INotifyPropertyChanged
    {
        public List<string> VM_ColumnList { get; }
        public PlotModel VM_SelectedPlot { get; }
        public PlotModel VM_CorrelatedPlot { get; }
        public PlotModel VM_AnomalyPlot { get; }
        public string VM_SelectedColumn { get; set; }
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

        public void CloseApplication();
    }
}
