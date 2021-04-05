using System.ComponentModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Adv_Prog_2
{
    class FlightGraphPlotter : IGraphPlotter
    {
        private string graphID = "";
        private PlotModel plotModel;
        private System.Timers.Timer refreshPlotsTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public PlotModel Plot 
        { 
            get { return plotModel; } 
            set 
            { 
                plotModel = value;
                NotifyPropertyChanged(graphID);
            }
        }

        public FlightGraphPlotter(string graphID)
        {
            this.graphID = graphID;
            plotModel = new PlotModel();
            this.refreshPlotsTimer = new System.Timers.Timer();
            plotModel.Background = OxyColors.Black;
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
        }

        // used so that the graph will have a static width
        // otherwise the axis labels can cause changes to the width
        public string AxisLabelFormatter(double value)
        {
            return "";
        }

        public void LoadData(float[] values, string title)
        {
            // delete all previous data points
            plotModel.Series.Clear();

            // create the data points
            LineSeries series = new LineSeries();
            for (int i = 0; i < values.Length; i++)
            {
                series.Points.Add(new DataPoint(i, values[i]));
            }

            // set the series title 
            series.Title = title;
            
            // add the series to the plot model
            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);

            // set the Model property
            this.Plot = plotModel;
        }

        // display linear regression
        public void LoadData(string title, float a, float b)
        {
            plotModel.Series.Clear();
            FunctionSeries series = new FunctionSeries(x => a * x + b, 0, 1, 0.1, "linear_reg");
            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);
            // y axis
            plotModel.Axes[0].TextColor = OxyColors.White;
            plotModel.Axes[0].LabelFormatter = null;
            this.Plot = plotModel;
        }

        public void SetDataCallback(string title, IFileIterator iterator, DataAnalyzer statistics)
        {
            const int DATAPOINTS_PER_GRAPH = 50;
            System.Timers.ElapsedEventHandler func =
                delegate (object sender, System.Timers.ElapsedEventArgs e)
                {
                    // get the values to display on our graph
                    float[] values = statistics.GetLastValues(title, iterator.LineNumber, DATAPOINTS_PER_GRAPH);
                    // plot the values (update the graph)
                    LoadData(values, title);
                };

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
