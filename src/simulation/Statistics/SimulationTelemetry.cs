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
        private static readonly List<RequestRecording> Data = new List<RequestRecording>();
        private static readonly List<string> Exceptions = new List<string>();
        private static readonly List<SimulationSnapshot> Snapshots = new List<SimulationSnapshot>();

        public void Add(RequestRecording recording)
        {
            Data.Add(recording);
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

        private void PrintReport(List<RequestRecording> requests, List<double> original, IList<double> withoutOutliers, List<string> exceptions, List<SimulationSnapshot> snapshots)
        {
            PrintSimulationSnapshots(snapshots, requests);
            PrintStatisticsTable(original, withoutOutliers);
            PrintStatusCodeTable(requests);
            PrintUrlTableByUrlFrequency(requests);
            PrintUrlTableByEndpointFrequency(requests);
            PrintUrlTableByUrlPerformance(requests);
            PrintUrlTableByEndpointPerformance(requests);
            PrintExceptions(exceptions);
        }

        private void PrintExceptions(List<string> exceptions)
        {
            var table = new ConsoleTable("exception");
            foreach (var exception in exceptions)
            {
                table.AddRow(exception);
            }
            table.Write(Format.MarkDown);
        }

        private static void PrintStatusCodeTable(List<RequestRecording> requests)
        {
            var statusCodeStats = requests.OrderBy(x => x.StatusCode)
                .GroupBy(x => x.StatusCode)
                .Select(x => new
                {
                    x.First().StatusCode,
                    Count = x.Count(),
                    Average = Stats.Mean(x.Select(m => (double)m.Elapsed)).Round()
                });

            var statusCodeTable = new ConsoleTable("status code", "count", "avg ms");

            foreach (var statusCodeStat in statusCodeStats)
            {
                statusCodeTable.AddRow(statusCodeStat.StatusCode, statusCodeStat.Count, statusCodeStat.Average);
            }
            statusCodeTable.Write(Format.MarkDown);
        }

        private static void PrintSimulationSnapshots(List<SimulationSnapshot> snapshots, List<RequestRecording> requests)
        {
            EnrichSnapshotsWithRequests(snapshots, requests);

            var buckets = Constants.DEFAULT_REPORT_BUCKET_SIZE;
            var snapshotsCount = snapshots.Count;
            var bucketSize = (int)Math.Floor((double)snapshotsCount / buckets);
            var duration = snapshots.Max(x => x.TimeStamp).Subtract(snapshots.Min(x => x.TimeStamp)).TotalSeconds;


            var table = new ConsoleTable("bucket", "requests", "avg ms", "95 percentile ms", "avg simulated speed (X)", "progress %", "duration s", "duration %");
            for (var i = 0; i < buckets; i++)
            {
                var bucket = snapshots.Skip(bucketSize * i).Take(bucketSize).ToList();
                var timings = new List<double>();

                foreach (var snapshot in bucket)
                {
                    var elapsed = snapshot.Requests.Select(x => (double)x.Elapsed).ToArray();
                    if (elapsed.Any())
                    {
                        timings.AddRange(elapsed);
                    }
                }

                var bucketDuration = bucket.Max(x => x.TimeStamp).Subtract(bucket.Min(x => x.TimeStamp)).TotalSeconds.Round();

                table.AddRow(i + 1,
                    bucket.Sum(x => x.Requests.Count),
                    Stats.Mean(timings).Round(),
                    Stats.Percentile(timings, 95).Round(),
                    Stats.Mean(bucket.Select(x => x.SimulatedSpeedMultiplier)).Round(),
                    Stats.Mean(bucket.Select(x => x.Progress)).ToString("P"),
                    bucketDuration,
                    (bucketDuration / duration).Round().ToString("P"));

            }
            table.Write(Format.MarkDown);
        }

        private static void EnrichSnapshotsWithRequests(List<SimulationSnapshot> snapshots, List<RequestRecording> requests)
        {
            var requestsDictionary = new Dictionary<DateTime, List<RequestRecording>>();

            foreach (var request in requests)
            {
                if (!requestsDictionary.ContainsKey(request.SimulatedDate))
                {
                    requestsDictionary.Add(request.SimulatedDate, new List<RequestRecording>());
                }
                requestsDictionary[request.SimulatedDate].Add(request);
            }

            foreach (var snapshot in snapshots)
            {
                snapshot.Requests = requestsDictionary.ContainsKey(snapshot.SimulatedDate)
                    ? requestsDictionary[snapshot.SimulatedDate]
                    : new List<RequestRecording>(0);
            }
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

        private static void PrintUrlTableByUrlFrequency(List<RequestRecording> requests)
        {
            var urls = requests
                .GroupBy(x => x.Url)
                .Select(x => new
                {
                    Url = x.First().Url,
                    Count = x.Count(),
                    Average = Stats.Mean(x.Select(m => ((double)m.Elapsed))).Round()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Url)
                .Take(Constants.DEFAULT_PRINT_TOP_REQUEST_COUNT);

            PrintUrlTable("request count by url", urls);
        }

        private static void PrintUrlTableByEndpointFrequency(List<RequestRecording> requests)
        {
            var urls = requests
                .GroupBy(x => x.Endpoint)
                .Select(x => new
                {
                    Url = x.First().Endpoint,
                    Count = x.Count(),
                    Average = Stats.Mean(x.Select(m => ((double)m.Elapsed))).Round()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Url)
                .Take(Constants.DEFAULT_PRINT_TOP_REQUEST_COUNT);

            PrintUrlTable("request count by endpoint", urls);
        }

        private static void PrintUrlTableByUrlPerformance(List<RequestRecording> requests)
        {
            var urls = requests
                .OrderByDescending(x => x.Elapsed)
                .ThenBy(x => x.Url)
                .GroupBy(x => x.Url)
                .Select(x => new
                {
                    Url = x.First().Url,
                    Count = x.Count(),
                    Average = Stats.Mean(x.Select(m => ((double)m.Elapsed))).Round()
                }).Take(Constants.DEFAULT_PRINT_TOP_REQUEST_COUNT);

            PrintUrlTable("performance by url", urls);
        }

        private static void PrintUrlTableByEndpointPerformance(List<RequestRecording> requests)
        {
            var urls = requests
                .OrderByDescending(x => x.Elapsed)
                .ThenBy(x => x.Endpoint)
                .GroupBy(x => x.Endpoint)
                .Select(x => new
                {
                    Url = x.First().Endpoint,
                    Count = x.Count(),
                    Average = Stats.Mean(x.Select(m => ((double)m.Elapsed))).Round()
                }).Take(Constants.DEFAULT_PRINT_TOP_REQUEST_COUNT);

            PrintUrlTable("performance by endpoint", urls);
        }

        private static void PrintUrlTable(string title, IEnumerable<dynamic> urls)
        {
            var urlsTable = new ConsoleTable(title, "count", "avg ms");
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

        public IList<RequestRecording> GetRequestData() => Data;

        public static SimulationTelemetry Instance { get; } = new SimulationTelemetry();
    }
}
