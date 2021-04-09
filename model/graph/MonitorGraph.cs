using System.ComponentModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Timers;

namespace Adv_Prog_2.model.graph
{
    class MonitorGraph : BaseGraph
    {
        public MonitorGraph(string graphID, DataAnalyzer dataAnalyzer, IFileIterator fileIterator)
            : base(graphID, dataAnalyzer, fileIterator) { }

        public void LoadData(float[] values, string title)
        {
            // delete all previous data points
            plotModel.Series.Clear();

            // create the data points
            LineSeries series = new LineSeries {
                Title = title,
                Color = OxyColors.DarkSlateGray
            };

            for (int i = 0; i < values.Length; i++)
            {
                series.Points.Add(new DataPoint(i, values[i]));
            }
            
            // add the series to the plot model
            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);

            // set the Model property
            Plot = plotModel;
        }

        // display linear regression
        public void LoadData(string title, float a, float b)
        {
            plotModel.Series.Clear();
            const float startX = 0.0f;
            const float endX = 1.0f;
            const float increment = 0.1f;
            FunctionSeries series = new FunctionSeries(x => a * x + b, startX, endX, increment, "linear_reg");
            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);
            // y axis
            plotModel.Axes[0].TextColor = OxyColors.White;
            plotModel.Axes[0].LabelFormatter = null;
            Plot = plotModel;
        }

        protected override ElapsedEventHandler GetCallback(string feature)
        {
            const int DATAPOINTS_PER_GRAPH = 100;
            return delegate (object sender, ElapsedEventArgs e)
                {
                    // get the values to display on our graph
                    float[] values = dataAnalyzer.GetLastValues(
                        feature, fileIterator.LineNumber, DATAPOINTS_PER_GRAPH, 0);
                    // plot the values (update the graph)
                    LoadData(values, feature);
                };
        } 

        public override void SetDataCallback(string feature)
        {
            ResetTimer(feature);
        }
    }
}
