using OxyPlot;
using OxyPlot.Series;
using System.Timers;
using Adv_Prog_2.model.data;

namespace Adv_Prog_2.model.graph
{
    class MonitorGraph : BaseGraph
    {
        #region ctor
        public MonitorGraph(DataAnalyzer dataAnalyzer, IFileIterator fileIterator)
            : base(dataAnalyzer, fileIterator) { }
        #endregion

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
            // reinitialize the timer
            ResetTimer(feature);
        }
    }
}
