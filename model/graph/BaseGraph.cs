using System.Collections.Generic;
using System.Timers;
using Adv_Prog_2.model.data;
using OxyPlot;
using OxyPlot.Axes;

namespace Adv_Prog_2.model.graph
{
    abstract class BaseGraph
    {
        // timer used to update the graphs every few ms
        public const int UPDATE_INTERVAL_MS = 150;
        protected Timer refreshPlotsTimer;
        // the actual PlotModel to display in the view
        protected PlotModel plotModel;
        // used to fetch the data to plot on the graph
        protected DataAnalyzer dataAnalyzer;
        protected IFileIterator fileIterator;

        public PlotModel Plot 
        { 
            get { return plotModel; }
            protected set { plotModel = value; }
        }

        #region ctor
        public BaseGraph(DataAnalyzer dataAnalyzer, IFileIterator fileIterator)
        {
            this.dataAnalyzer = dataAnalyzer;
            this.fileIterator = fileIterator;
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
        #endregion

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

        protected abstract ElapsedEventHandler GetCallback(string feature);

        // let the subclasses set the initial drawing
        // and setting up the callback function that draws
        // new plots every few ms
        public abstract void SetDataCallback(string feature);
        
        public void StopCallbackTimer()
        {
            // stop updating the graph
            refreshPlotsTimer.Stop();
        }

        public void StartCallbackTimer()
        {
            // start updating the graph
            refreshPlotsTimer.Start();
        }
    }
}
