using System.ComponentModel;
using OxyPlot;

namespace Adv_Prog_2
{
    interface IGraphPlotter : INotifyPropertyChanged
    {
        public PlotModel Plot { get; }
        public void LoadData(float[] values, string title);
        public void LoadData(string title, float a, float b);
        public void SetDataCallback(string title, IFileIterator iterator, DataAnalyzer statistics);
        public void StopCallbackTimer();
        public void StartCallbackTimer();
    }
}
