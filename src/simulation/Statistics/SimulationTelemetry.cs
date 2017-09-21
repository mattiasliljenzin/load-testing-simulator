using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTables;
using RequestSimulation.Extensions;
using RequestSimulation.Requests;
using Stats = MathNet.Numerics.Statistics.Statistics;

namespace RequestSimulation.Statistics
{
    public class SimulationTelemetry
    {
        private static readonly List<RequestData> Data = new List<RequestData>();
        private static readonly List<string> Exceptions = new List<string>();
        private static readonly List<SimulationSnapshot> Snapshots = new List<SimulationSnapshot>();

        public void Add(RequestData data)
        {
            Data.Add(data);
        }

        public void Add(SimulationSnapshot snapshot)
        {
            Snapshots.Add(snapshot);
        }

        public void PrintReport()
        {
            var original = Data.Select(x => (double)x.Elapsed).ToList();
            var noOutliers = FilterOutliers(original);

            PrintReport(Data, original, noOutliers, Exceptions, Snapshots);
        }

        private void PrintReport(List<RequestData> requests, List<double> original, IList<double> withoutOutliers, List<string> exceptions, List<SimulationSnapshot> snapshots)
        {
            PrintSimulationSnapshots(snapshots);
            PrintStatisticsTable(original, withoutOutliers);
            PrintStatusCodeTable(requests);
            PrintUrlTable(requests);
            PrintExceptions(exceptions);
        }

        private void PrintExceptions(List<string> exceptions)
        {
            var table = new ConsoleTable("message");
            foreach (var exception in exceptions)
            {
                table.AddRow(exception);
            }
            table.Write(Format.MarkDown);
        }

        private static void PrintStatusCodeTable(List<RequestData> requests)
        {
            var statusCodeStats = requests.OrderBy(x => x.StatusCode)
                .GroupBy(x => x.StatusCode)
                .Select(x => new
                {
                    x.First().StatusCode,
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

        private static void PrintSimulationSnapshots(List<SimulationSnapshot> snapshots)
        {
            var bucketSize = 10;
            var snapshotsCount = snapshots.Count;
            var iterations = Math.Ceiling((double)(snapshotsCount / bucketSize));

            var table = new ConsoleTable("bucket", "req/s", "simulated speed (X)");
            for (var i = 1; i <= iterations; i++)
            {
                var bucket = snapshots.Skip(bucketSize * i).Take(bucketSize).ToList();
                table.AddRow(i, bucket.Average(x => x.RequestPerSeconds), bucket.Average(x => x.SimulatedSpeedMultiplier));
            }
            table.Write(Format.MarkDown);
        }

        private IList<double> FilterOutliers(IList<double> data)
        {
            var k = 1.5; // tukey constant
            var iqr = Stats.InterquartileRange(data);
            var q1 = Stats.LowerQuartile(data);
            var q3 = Stats.UpperQuartile(data);
            var outlierLow = q1 - k * iqr;
            var outlierHigh = q3 + k * iqr;

            return data.Where(x => x >= outlierLow && x <= outlierHigh).ToList();
        }

        internal void AddException(ISimulatedRequest request, Exception ex)
        {
            Exceptions.Add($"{request.Uri} - {ex.Message}");
        }

        private static void PrintUrlTable(List<RequestData> requests)
        {
            var urls = requests.OrderBy(x => x.Url)
                .GroupBy(x => x.Url)
                .Select(x => new
                {
                    Url = x.First().Url,
                    Count = x.Count(),
                    Average = Stats.Mean(x.Select(m => ((double)m.Elapsed))).Round()
                });

            var urlsTable = new ConsoleTable("url", "count", "avg ms");
            foreach (var url in urls)
            {
                urlsTable.AddRow(url.Url, url.Count, url.Average);
            }
            urlsTable.Write(Format.MarkDown);
        }

        private static void PrintStatisticsTable(IList<double> data, IList<double> withoutOutliers)
        {
            var table = new ConsoleTable("title", "original", "no outliers");
            table.AddRow("Count", data.Count(), withoutOutliers.Count());
            table.AddRow("Maximum", Stats.Maximum(data).Round(), Stats.Maximum(withoutOutliers).Round());
            table.AddRow("Minimum", Stats.Minimum(data).Round(), Stats.Minimum(withoutOutliers).Round());
            table.AddRow("Mean", Stats.Mean(data).Round(), Stats.Mean(withoutOutliers).Round());
            table.AddRow("Median", Stats.Median(data).Round(), Stats.Median(withoutOutliers).Round());
            table.AddRow("Variance", Stats.Variance(data).Round(), Stats.Variance(withoutOutliers).Round());
            table.AddRow("StandardDeviation", Stats.StandardDeviation(data).Round(),
                Stats.StandardDeviation(withoutOutliers).Round());
            table.AddRow("Percentile 90", Stats.Percentile(data, 90).Round(), Stats.Percentile(withoutOutliers, 90).Round());
            table.AddRow("Percentile 95", Stats.Percentile(data, 95).Round(), Stats.Percentile(withoutOutliers, 95).Round());
            table.AddRow("Percentile 99", Stats.Percentile(data, 99).Round(), Stats.Percentile(withoutOutliers, 99).Round());
            table.Write(Format.MarkDown);
        }


        public long RequestCount => Data.Count;

        public static SimulationTelemetry Instance { get; } = new SimulationTelemetry();
    }
}
