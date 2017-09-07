using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace simulation
{
    public class RequestScheduler : ISimulationSubscriber
    {
        private IRequestDataSource _requestSourceService;
        private IDictionary<DateTime, IList<ISimulatedRequest>> _simulatedRequests;
        readonly IRequestExecutor _requestExecutor;

        public RequestScheduler(IRequestDataSource requestSourceService, IRequestExecutor requestExecutor)
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

            System.Console.WriteLine($"[RequestScheduler]: Found {matchingRequests?.Count} matching requests for {simulatedDate}");

            foreach (var request in matchingRequests)
            {
                _requestExecutor.Execute(request);
            }

            return Task.CompletedTask;
        }
    }
}
