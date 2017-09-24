    using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using RequestSimulation.Executing.Interceptors;
using RequestSimulation.Extensions;
using RequestSimulation.Requests;
using RequestSimulation.Statistics;

namespace RequestSimulation.Executing
{
    public class RequestExecutor : IRequestExecutor
    {
        private readonly IHttpRequestMessageInterceptor[] _interceptors;
        private readonly HttpClient _client = new HttpClient();
        private Random random = new Random();

        public RequestExecutor(IHttpRequestMessageInterceptor[] interceptors)
        {
            _interceptors = interceptors;
        }

        public async Task Execute(ISimulatedRequest request)
        {
            try
            {
                var message = new HttpRequestMessage(HttpMethod.Get, request.Uri);
                foreach (var interceptor in _interceptors)
                {
                    await interceptor.InterceptAsync(message);
                }

                Console.WriteLine($"Executing: {message.RequestUri}");
                var timer = new Stopwatch();
                timer.Start();

                //var response = await _client.SendAsync(message);

                await Task.Delay(random.Next(100, 1000));

                timer.Stop();

                var metric = new RequestData
                {
                    Elapsed = timer.ElapsedMilliseconds,
                    Endpoint = message.RequestUri.Host,
                    StatusCode = 200, //(int) response.StatusCode,
                    Url = message.RequestUri.ToString(),
                    SimulatedDate = request.Created.Normalize()
                };

                SimulationTelemetry.Instance.Add(metric);
            }
            catch (Exception ex)
            {
                SimulationTelemetry.Instance.AddException(request, ex);
            }
        }
    }
}
