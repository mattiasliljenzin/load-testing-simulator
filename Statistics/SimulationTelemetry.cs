using System;
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

            PrintReport(_data, original, noOutliers);
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

        private void PrintReport(List<RequestData> requests, IEnumerable<double> data, IEnumerable<double> withoutOutliers)
        {
            var table = new ConsoleTable("title", "original", "no outliers");
			table.AddRow("Maximum", Stats.Maximum(data).Round(), Stats.Maximum(withoutOutliers).Round());
			table.AddRow("Minimum", Stats.Minimum(data).Round(), Stats.Minimum(withoutOutliers).Round());
			table.AddRow("Mean", Stats.Mean(data).Round(), Stats.Mean(withoutOutliers).Round());
			table.AddRow("Median", Stats.Median(data).Round(), Stats.Median(withoutOutliers).Round());
			table.AddRow("Variance", Stats.Variance(data).Round(), Stats.Variance(withoutOutliers).Round());
			table.AddRow("StandardDeviation", Stats.StandardDeviation(data).Round(), Stats.StandardDeviation(withoutOutliers).Round());
			table.AddRow("Percentile 90", Stats.Percentile(data, 90).Round(), Stats.Percentile(withoutOutliers, 90).Round());
			table.AddRow("Percentile 95", Stats.Percentile(data, 95).Round(), Stats.Percentile(withoutOutliers, 95).Round());
			table.AddRow("Percentile 99", Stats.Percentile(data, 99).Round(), Stats.Percentile(withoutOutliers, 99).Round());
			table.Write(Format.MarkDown);

            var statusCodeStats = requests.OrderBy(x => x.StatusCode)
                                          .GroupBy(x => x.StatusCode)
                                          .Select(x => new
                                          {
                                              StatusCode = x.First().StatusCode,
                                              Count = x.Count(),
                                              Average = Stats.Mean(x.Select(m => ((double)m.Elapsed))).Round()
                                          });

			var statusCodeTable = new ConsoleTable("status code", "count", "avg ms");
			foreach (var statusCodeStat in statusCodeStats)
            {
                statusCodeTable.AddRow(statusCodeStat.StatusCode, statusCodeStat.Count, statusCodeStat.Average);
            }
            statusCodeTable.Write(Format.MarkDown);
        }



        public long RequestCount => _data.Count;


        public static SimulationTelemetry Instance => _instance;
    }
}
