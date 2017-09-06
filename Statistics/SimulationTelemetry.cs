using System.Collections.Generic;
using System.Linq;
using ConsoleTables;
using Stats = MathNet.Numerics.Statistics.Statistics;

namespace simulation.Statistics
{
    public class SimulationTelemetry
    {
        private readonly static List<RequestData> _data = new List<RequestData>();
        private readonly static SimulationTelemetry _instance = new SimulationTelemetry();

        public void Add(RequestData data)
        {
            _data.Add(data);
        }

        public void PrintReport()
        {
            var original = _data.Select(x => (double)x.Elapsed).ToList();
            var noOutliers = FilterOutliers(original);

            PrintReport(original, noOutliers);
        }

        private IEnumerable<double> FilterOutliers(IEnumerable<double> data)
        {
            var k = 1.5; // tukey constant
            var iqr = Stats.InterquartileRange(data);
            var q1 = Stats.LowerQuartile(data);
            var q3 = Stats.UpperQuartile(data);
            var outlierLow = q1 - k * iqr;
            var outlierHigh = q3 + k * iqr;

            return data.Where(x => x >= outlierLow && x <= outlierHigh).ToList();
        }

        private void PrintReport(IEnumerable<double> data, IEnumerable<double> withoutOutliers)
        {
            var table = new ConsoleTable("title", "original", "no outliers");
            table.AddRow("Maximum", Stats.Maximum(data), Stats.Maximum(withoutOutliers));
            table.AddRow("Minimum", Stats.Minimum(data), Stats.Minimum(withoutOutliers));
            table.AddRow("Mean", Stats.Mean(data), Stats.Mean(withoutOutliers));
            table.AddRow("Median", Stats.Median(data), Stats.Median(withoutOutliers));
            table.AddRow("Variance", Stats.Variance(data), Stats.Variance(withoutOutliers));
            table.AddRow("StandardDeviation", Stats.StandardDeviation(data), Stats.StandardDeviation(withoutOutliers));
            table.AddRow("Percentile 90", Stats.Percentile(data, 90), Stats.Percentile(withoutOutliers, 90));
            table.AddRow("Percentile 95", Stats.Percentile(data, 95), Stats.Percentile(withoutOutliers, 95));
            table.AddRow("Percentile 99", Stats.Percentile(data, 99), Stats.Percentile(withoutOutliers, 99));

            table.Write();
        }

        public long RequestCount => _data.Count;


        public static SimulationTelemetry Instance => _instance;
    }
}
