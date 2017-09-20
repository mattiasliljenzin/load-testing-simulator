using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequestSimulation.Datasources;
using RequestSimulation.Requests;
using System.Linq;
using ConsoleTables;

namespace RequestSimulation.Executing
{
    public class RequestDelegator : ISimulationSubscriber
    {
        private readonly IRequestDataSource _requestSourceService;
        private IDictionary<DateTime, IList<ISimulatedRequest>> _simulatedRequests;
        readonly IRequestExecutor _requestExecutor;

        public RequestDelegator(IRequestDataSource requestSourceService, IRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
            _requestSourceService = requestSourceService;
        }

        public async Task PopulateRequestsAsync(DateTime from, DateTime to)
        {
            _simulatedRequests = await _requestSourceService.GetAsync(from, to);
            PrintTopRequestsCollected(_simulatedRequests);
        }

        private void PrintTopRequestsCollected(IDictionary<DateTime, IList<ISimulatedRequest>> simulatedRequests)
        {
            var list = new List<ISimulatedRequest>();
            foreach (var item in simulatedRequests.Values)
            {
                list.AddRange(item);
            }

            var requests = list.GroupBy(x => x.Uri).Select(x =>
            new
            {
                Url = x.First().Uri,
                Count = x.Count()
            });

            var requestTable = new ConsoleTable("url", "count");
            foreach (var request in requests.OrderByDescending(x => x.Count).Take(Constants.DEFAULT_PRINT_TOP_REQUEST_COUNT))
            {
                requestTable.AddRow(request.Url, request.Count);
            }
            requestTable.Write(Format.MarkDown);

        }

        public Task OnPublish(DateTime simulatedDate)
        {
            var matchingRequests = _simulatedRequests.ContainsKey(simulatedDate) ?
                _simulatedRequests[simulatedDate] :
                new ISimulatedRequest[0];

            Console.WriteLine($"[RequestDelegator]: Found {matchingRequests.Count} matching requests for {simulatedDate}");

            foreach (var request in matchingRequests)
            {
                _requestExecutor.Execute(request);
            }

            return Task.CompletedTask;
        }
    }
}
