using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using OxyPlot;
using OxyPlot.Series;

namespace Adv_Prog_2.model.graph
{
    class AnomalyGraph : BaseGraph
    {
        private LineSeries pointSeries;
        private IList<DataPoint> points;
        private string lastFeature = "";

        public AnomalyGraph(string graphID, DataAnalyzer dataAnalyzer, IFileIterator fileIterator)
            : base(graphID, dataAnalyzer, fileIterator) 
        {
            points = new List<DataPoint>();
            pointSeries = new LineSeries {
                Title = "recent data samples",
                ItemsSource = points,
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                Color = OxyColors.SlateGray,
                MarkerFill = OxyColors.SlateGray,
            };
            plotModel.Series.Add(pointSeries);

            // y axis
            plotModel.Axes[0].TextColor = OxyColors.White;
            plotModel.Axes[0].LabelFormatter = null;
            // x axis
            plotModel.Axes[1].TextColor = OxyColors.White;
            plotModel.Axes[1].LabelFormatter = null;
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
            Plot = plotModel;
        }

        public void AddAlgorithm(string feature)
        {
            // get the linear regression function (f(x) = a * x + b)
            float a, b;
            dataAnalyzer.GetLinearReg(feature, out a, out b);

            // clear the plot
            plotModel.Series.Clear();
            
            // draw the linear regression between within the current range of the x axis
            double startX = plotModel.Axes[1].Minimum;
            double endX = plotModel.Axes[1].Maximum;
            double increment = endX;
            FunctionSeries series = new FunctionSeries(x => a * x + b, startX, endX, increment, "linear_reg");
            series.Color = OxyColors.DarkSlateGray;
            plotModel.Series.Add(series);
        }

        protected void AddLastDataPoints(string feature)
        {
            // clear the points we entered previously
            points.Clear();

            const int DATAPOINTS = 30;
            int lineNum = fileIterator.LineNumber;
            // align lineNum with the skip value
            lineNum -= lineNum % 10;
            string correlated = dataAnalyzer.GetCorrelatedColumn(feature);

            float minX = float.MaxValue;
            float maxX = float.MinValue;

            float[] featureValues = dataAnalyzer.GetLastValues(
                feature, lineNum, DATAPOINTS, 9);
            float[] correlatedValues = dataAnalyzer.GetLastValues(
                correlated, lineNum, DATAPOINTS, 9);

            // create datapoints
            for (int i = 0; i < DATAPOINTS; i++)
            {
                points.Add(new DataPoint(featureValues[i], correlatedValues[i]));
                minX = Math.Min(minX, featureValues[i]);
                maxX = Math.Max(maxX, featureValues[i]);
            }
        }

        protected override ElapsedEventHandler GetCallback(string feature)
        {
            return delegate (object sender, ElapsedEventArgs e)
            {
                AddLastDataPoints(feature);
                pointSeries.ItemsSource = points;
                plotModel.Series.Remove(pointSeries);
                plotModel.Series.Add(pointSeries);
                plotModel.InvalidatePlot(true);
                Plot = plotModel;
            };
        }

        public override void SetDataCallback(string feature)
        {
            // don't reset the graph if it's already displaying 'feature'
            if (lastFeature != feature)
            {
                lastFeature = feature;
                // set the axes values according to 'feature' and its correlated feature
                SetAxesMinMax(feature);
                // draw the algorithm (e.g. linear regression, circle, etc.)
                AddAlgorithm(feature);
                // set the timer to update automatically with 'feature' values
                ResetTimer(feature);
            }
        }
    }
}
