using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace simulation
{
    public class GeneratedRequestDataSource : IRequestDataSource
    {
        private readonly int _numberOfRequestsToGenerate;
        private static readonly Random _random = new Random();


        public GeneratedRequestDataSource(int numberOfRequestsToGenerate = 25)
        {
            _numberOfRequestsToGenerate = numberOfRequestsToGenerate;
        }

        public Task<IDictionary<DateTime, IList<ISimulatedRequest>>> GetAsync(DateTime from, DateTime to)
        {
            from = from.Normalize();
            to = to.Normalize();

            var diff = to.Subtract(from).TotalMilliseconds;
            var delta = _random.Next(1, (int)diff);

            Func<DateTime> randomizeDate = () => from.AddMilliseconds(_random.Next(1, (int)diff)).Normalize();

            var requests = Enumerable
                .Range(0, _numberOfRequestsToGenerate)
                .Select(x => SimulatedRequest.Create("http://localhost", $"/test/{x}", null, Method.GET, randomizeDate()))
                .OrderBy(x => x.Created)
                .ToList();

            var uniqueDates = requests.Select(x => x.Created).Distinct();
            var result = new Dictionary<DateTime, ISimulatedRequest[]>();

            foreach (var request in requests)
            {
                System.Console.WriteLine($"[HardCodedRequestSourceService]: Added request: {request.ToString()}");
            }

            foreach (var date in uniqueDates)
            {
                var match = requests.Where(x => x.Created == date).ToArray();
                System.Console.WriteLine($"[HardCodedRequestSourceService]: Added matching request count of {match.Length} for {date}");

                result.Add(date, match);
            }
            return Task.FromResult(result as IDictionary<DateTime, IList<ISimulatedRequest>>);
        }

        
    }

}
