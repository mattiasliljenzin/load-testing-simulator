using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequestSimulation.Datasources;
using RequestSimulation.Requests;

namespace RequestSimulation.Executing
{
    public class RequestMediator : ISimulationSubscriber
    {
        private readonly IRequestDataSource _requestSourceService;
        private IDictionary<DateTime, IList<ISimulatedRequest>> _simulatedRequests;
        readonly IRequestExecutor _requestExecutor;

        public RequestMediator(IRequestDataSource requestSourceService, IRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
            _requestSourceService = requestSourceService;
        }

        public async Task PopulateRequestsAsync(DateTime from, DateTime to)
        {
            _simulatedRequests = await _requestSourceService.GetAsync(from, to);
        }

        public Task OnPublish(DateTime simulatedDate)
        {
            var matchingRequests = _simulatedRequests.ContainsKey(simulatedDate) ?
                _simulatedRequests[simulatedDate] :
                new ISimulatedRequest[0];

            Console.WriteLine($"[RequestMediator]: Found {matchingRequests.Count} matching requests for {simulatedDate}");

            foreach (var request in matchingRequests)
            {
                _requestExecutor.Execute(request);
            }

            return Task.CompletedTask;
        }
    }
}
