using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RequestSimulation.Extensions;
using RequestSimulation.Requests;

namespace RequestSimulation.Datasources
{
    public class GeneratedRequestDataSource : IRequestDataSource
    {
        private readonly int _numberOfRequestsToGenerate;
        private static readonly Random Random = new Random();

        public GeneratedRequestDataSource(int numberOfRequestsToGenerate = 25)
        {
            _numberOfRequestsToGenerate = numberOfRequestsToGenerate;
        }

        public Task<IDictionary<DateTime, IList<ISimulatedRequest>>> GetAsync(DateTime from, DateTime to)
        {
            from = from.Normalize();
            to = to.Normalize();

            var diff = to.Subtract(from).TotalMilliseconds;
            var delta = Random.Next(1, (int)diff);

            DateTime RandomizeDate() => from.AddMilliseconds(delta).Normalize();

            var requests = Enumerable
                .Range(0, _numberOfRequestsToGenerate)
                .Select(x => SimulatedRequest.Create("http://localhost", $"/test/{x}", null, "GET", RandomizeDate()))
                .OrderBy(x => x.Created)
                .ToList();

            var uniqueDates = requests.Select(x => x.Created).Distinct();
            var result = new Dictionary<DateTime, ISimulatedRequest[]>();

            foreach (var request in requests)
            {
                Console.WriteLine($"[HardCodedRequestSourceService]: Added request: {request}");
            }

            foreach (var date in uniqueDates)
            {
                var match = requests.Where(x => x.Created == date).ToArray();
                Console.WriteLine($"[HardCodedRequestSourceService]: Added matching request count of {match.Length} for {date}");

                result.Add(date, match);
            }
            return Task.FromResult(result as IDictionary<DateTime, IList<ISimulatedRequest>>);
        }

        
    }

}
