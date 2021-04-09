using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using OxyPlot;
using OxyPlot.Axes;

namespace Adv_Prog_2.model.graph
{
    abstract class BaseGraph : INotifyPropertyChanged
    {
        public const int UPDATE_INTERVAL_MS = 150;
        private string graphID;
        protected PlotModel plotModel;
        protected Timer refreshPlotsTimer;
        protected DataAnalyzer dataAnalyzer;
        protected IFileIterator fileIterator;

        public IList<DataPoint> Points { get; protected set; }

        public PlotModel Plot 
        { 
            get
            {
                return plotModel;
            }
            protected set
            {
                plotModel = value;
                NotifyPropertyChanged(graphID);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public BaseGraph(string graphID, DataAnalyzer dataAnalyzer, IFileIterator fileIterator)
        {
            this.dataAnalyzer = dataAnalyzer;
            this.fileIterator = fileIterator;
            this.graphID = graphID;
            plotModel = new PlotModel();
            this.refreshPlotsTimer = new Timer();
            plotModel.TextColor = OxyColors.White;
            // y axis
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                LabelFormatter = AxisLabelFormatter,
                TextColor = OxyColors.Transparent,
                TickStyle = TickStyle.None,
            });
            // x axis
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                LabelFormatter = AxisLabelFormatter,
                TextColor = OxyColors.Transparent,
                TickStyle = TickStyle.None,
            });

            Plot = plotModel;
        }

        // used so that the graph will have a static width
        // otherwise the axis labels can cause changes to the width
        public string AxisLabelFormatter(double value)
        {
            return "";
        }

        protected void ResetTimer(string feature)
        {
            // get rid of the previous timer
            refreshPlotsTimer.Stop();
            refreshPlotsTimer.Close();

            // initialize a new timer
            refreshPlotsTimer = new Timer();
            refreshPlotsTimer.Interval = UPDATE_INTERVAL_MS;

            // call func every UPDATE_INTERVAL milliseconds
            refreshPlotsTimer.Elapsed += GetCallback(feature);

            StartCallbackTimer();
        }

        protected void SetAxesMinMax(string feature)
        {
            // feature's values are in the x axis
            plotModel.Axes[1].Minimum = dataAnalyzer.GetMin(feature);
            plotModel.Axes[1].Maximum = dataAnalyzer.GetMax(feature);

            // correlated-feature's values are in the y axis
            string correlated = dataAnalyzer.GetCorrelatedColumn(feature);
            plotModel.Axes[0].Minimum = dataAnalyzer.GetMin(correlated);
            plotModel.Axes[0].Maximum = dataAnalyzer.GetMax(correlated);

            // make sure we don't get a zero length axis (otherwise oxyplot will throw an error)
            if (plotModel.Axes[1].Minimum == plotModel.Axes[1].Maximum)
            {
                plotModel.Axes[1].Maximum = plotModel.Axes[1].Minimum + 1;
            }
            if (plotModel.Axes[0].Minimum == plotModel.Axes[0].Maximum)
            {
                plotModel.Axes[0].Maximum = plotModel.Axes[0].Minimum + 1;
            }
        }

        protected abstract ElapsedEventHandler GetCallback(string feature);

        public abstract void SetDataCallback(string feature);
        
        public void StopCallbackTimer()
        {
            refreshPlotsTimer.Stop();
        }

        public void StartCallbackTimer()
        {
            refreshPlotsTimer.Start();
        }
    }
}
