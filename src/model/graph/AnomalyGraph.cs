using System;
using System.Diagnostics;
using System.Timers;
using Adv_Prog_2.model.data;
using OxyPlot;
using OxyPlot.Series;

namespace Adv_Prog_2.model.graph
{
    class AnomalyGraph : BaseGraph
    {
        private AnomalyAnalyzer anomalyAnalyzer;
        private LineSeries pointSeries;
        private LineSeries anomalySeries;

        #region ctor
        public AnomalyGraph(DataAnalyzer dataAnalyzer, 
            IFileIterator fileIterator, AnomalyAnalyzer anomalyAnalyzer)
            : base(dataAnalyzer, fileIterator) 
        {
            // use an anomaly analyzer to handle anomaly related functions
            this.anomalyAnalyzer = anomalyAnalyzer;

            // initialize lineseries for recent points and anomaly points 
            // style with LineStyle=None
            const int MARKER_SIZE = 3;
            pointSeries = new LineSeries {
                Title = "recent data samples",
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle,
                MarkerSize = MARKER_SIZE,
                Color = OxyColors.SlateGray,
                MarkerFill = OxyColors.SlateGray,
            };
            plotModel.Series.Add(pointSeries);

            anomalySeries = new LineSeries
            {
                Title = "anomalies",
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle,
                MarkerSize = MARKER_SIZE,
                Color = OxyColor.FromRgb(255, 128, 128),
                MarkerFill = OxyColor.FromRgb(255, 128, 128),
            };
            plotModel.Series.Add(anomalySeries);

            // initialize the axes
            // y axis
            plotModel.Axes[0].TextColor = OxyColors.White;
            plotModel.Axes[0].LabelFormatter = null;
            // x axis
            plotModel.Axes[1].TextColor = OxyColors.White;
            plotModel.Axes[1].LabelFormatter = null;
        }
        #endregion

        private void IncreaseAxesEdges(float multiplier)
        {
            // increase the length by 10% on each side
            double yDiff = plotModel.Axes[0].Maximum - plotModel.Axes[0].Minimum;
            plotModel.Axes[0].Minimum -= yDiff * multiplier;
            plotModel.Axes[0].Maximum += yDiff * multiplier;
            double xDiff = plotModel.Axes[1].Maximum - plotModel.Axes[1].Minimum;
            plotModel.Axes[1].Minimum -= xDiff * multiplier;
            plotModel.Axes[1].Maximum += xDiff * multiplier;
        }

        // make sure the algorithm plot is fully inside the screen
        private void SetAxesMinMax(string feature, string correlated, float[] funcData)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            if (funcData != null && funcData.Length % 2 == 0)
            {
                int half = funcData.Length / 2;
                // find the min/max x/y values 
                for (int i = 0; i < half; i++)
                {
                    float x = funcData[2 * i];
                    minX = Math.Min(minX, x);
                    maxX = Math.Max(maxX, x);
                    float y = funcData[2 * i + 1];
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);
                }
            }

            // feature's values are in the x axis
            // correlated-feature's values are in the y axis
            plotModel.Axes[0].Minimum = Math.Min(minY, dataAnalyzer.GetMin(correlated));
            plotModel.Axes[0].Maximum = Math.Max(maxY, dataAnalyzer.GetMax(correlated));
            plotModel.Axes[1].Minimum = Math.Min(minX, dataAnalyzer.GetMin(feature));
            plotModel.Axes[1].Maximum = Math.Max(maxX, dataAnalyzer.GetMax(feature));

            // make sure we don't get a zero length axis (otherwise oxyplot will throw an error)
            if (plotModel.Axes[1].Maximum == plotModel.Axes[1].Minimum)
            {
                plotModel.Axes[1].Maximum = plotModel.Axes[1].Minimum + 1;
            }
            if (plotModel.Axes[0].Maximum == plotModel.Axes[0].Minimum)
            {
                plotModel.Axes[0].Maximum = plotModel.Axes[0].Minimum + 1;
            }

            // increase the length by 10% of the axes
            // otherwise the plots might be not be fully in the screen
            IncreaseAxesEdges(0.1f);
        }

        private void AddAlgorithm(float[] funcData)
        {
            if (funcData != null)
            {
                // funcData contains x,y value pairs in adjacent cells
                // therefore it must contain even number of elements
                if (funcData.Length % 2 != 0) { return; }
                // load all the datapoints 'series'
                LineSeries series = new LineSeries();
                int half = funcData.Length / 2;
                for (int i = 0; i < half; i++)
                {
                    float x = funcData[2 * i];
                    float y = funcData[2 * i + 1];
                    series.Points.Add(new DataPoint(x, y));
                }
                series.Color = OxyColors.DarkSlateGray;
                plotModel.Series.Add(series);
            }
        }

        // simple binary search to determine if 'line' is an anomaly
        // assumes that 'timeSteps' is sorted in increasing order
        private bool TimeStepContains(int[] timeSteps, int line)
        {
            int start = 0;
            int end = timeSteps.Length - 1;
            while (start <= end)
            {
                int mid = (end + start) / 2;
                int timeStep = timeSteps[mid];
                if (timeStep == line)
                {
                    return true;
                }
                if (timeStep < line)
                {
                    start = mid + 1;
                }
                else if (timeStep > line)
                {
                    end = mid - 1;
                }
            }
            return false;
        }

        // add recent datapoints (and anomalies if there are any) to the graph
        private void AddDataPoints(string feature, string correlated, int[] timeSteps)
        {
            // clear any points that are currently on the graph
            pointSeries.Points.Clear();
            anomalySeries.Points.Clear();

            // we don't want to display every single point in the last few seconds
            // so we skip every few points
            const int SKIP = 10;
            const int DATAPOINTS_TO_DISPLAY = 30;
            int currLine = fileIterator.LineNumber;
            // align currLine
            currLine -= currLine % SKIP;
            // set the starting line of which we're going to look for data points
            // of course it cannot be lower than 0
            int lowestLine = Math.Max(currLine - DATAPOINTS_TO_DISPLAY * SKIP, 0);

            for (int i = lowestLine; i <= currLine; i += SKIP)
            {
                // get the datapoint of the current line
                float x = dataAnalyzer.GetValue(feature, i);
                float y = dataAnalyzer.GetValue(correlated, i);
                DataPoint point = new DataPoint(x, y);
                pointSeries.Points.Add(point);
                if (timeSteps != null && timeSteps.Length > 0)
                {
                    // if current line is an anomaly - add it to anomalySeries
                    if (TimeStepContains(timeSteps, i))
                    {
                        anomalySeries.Points.Add(point);
                    }
                }
            }
        }

        protected override ElapsedEventHandler GetCallback(string feature)
        {
            int[] timeSteps = anomalyAnalyzer.GetAnomalyTimeSteps(feature);
            string correlated = dataAnalyzer.GetCorrelatedColumn(feature);
            return delegate (object sender, ElapsedEventArgs e)
            {
                if (correlated != null)
                {
                    // add all recent datapoints (and/or anomalies)
                    AddDataPoints(feature, correlated, timeSteps);
                }
                // reload the point series into our plotModel
                plotModel.Series.Remove(pointSeries);
                plotModel.Series.Add(pointSeries);
                plotModel.Series.Remove(anomalySeries);
                plotModel.Series.Add(anomalySeries);
                // tell oxyplot to refresh the graph in the view
                plotModel.InvalidatePlot(true);
            };
        }

        public override void SetDataCallback(string feature)
        {
            float[] funcData = anomalyAnalyzer.GetFunctionData(feature);
            string correlated = dataAnalyzer.GetCorrelatedColumn(feature);
            if (correlated != null)
            {
                // set the axes values according to 'feature' and its correlated feature
                SetAxesMinMax(feature, correlated, funcData);
            }
            // clear the plot
            plotModel.Series.Clear();
            // draw the algorithm (e.g. linear regression, circle, etc.)
            AddAlgorithm(funcData);
            // set the timer to update automatically with 'feature' values
            ResetTimer(feature);
        }
    }
}
