using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Adv_Prog_2
{
    class FlightGraphPlotter : IGraphPlotter
    {
        private PlotModel plotModel;
        private System.Timers.Timer refreshPlotsTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public PlotModel Plot { get; set; }

        public FlightGraphPlotter()
        {
            plotModel = new PlotModel();
            this.refreshPlotsTimer = new System.Timers.Timer();
            // y axis
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                TextColor = OxyColors.Transparent,
                TickStyle = TickStyle.None
            });
            // x axis
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TextColor = OxyColors.Transparent,
                TickStyle = TickStyle.None
            });
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void LoadData(float[] values)
        {
            // delete all previous data points
            plotModel.Series.Clear();

            // create the data points
            LineSeries series = new LineSeries();
            for (int i = 0; i < values.Length; i++)
            {
                series.Points.Add(new DataPoint(i, values[i]));
            }
            // Add the series to the plot model
            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);

            // Set the Model property
            this.Plot = plotModel;
            NotifyPropertyChanged("GraphPlot");
        }

        public void setDataCallback(System.Timers.ElapsedEventHandler func)
        {
            // get rid of the previous timer
            this.refreshPlotsTimer.Stop();
            this.refreshPlotsTimer.Close();
            // initialize a new timer
            const int UPDATE_INTERVAL = 250;
            this.refreshPlotsTimer = new System.Timers.Timer();
            this.refreshPlotsTimer.Interval = UPDATE_INTERVAL;
            // call func every UPDATE_INTERVAL milliseconds
            this.refreshPlotsTimer.Elapsed += func;
            StartCallbackTimer();
        }

        public void StopCallbackTimer()
        {
            this.refreshPlotsTimer.Stop();
        }

        public void StartCallbackTimer()
        {
            this.refreshPlotsTimer.Start();
        }
    }
}
