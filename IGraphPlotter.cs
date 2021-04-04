using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using OxyPlot;

namespace Adv_Prog_2
{
    interface IGraphPlotter : INotifyPropertyChanged
    {
        public PlotModel Plot { get; }
        public void LoadData(float[] values);
        public void setDataCallback(System.Timers.ElapsedEventHandler func);
        public void StopCallbackTimer();
        public void StartCallbackTimer();
    }
}
