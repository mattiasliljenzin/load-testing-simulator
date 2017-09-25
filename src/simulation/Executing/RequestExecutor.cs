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
                    interceptor.InterceptAsync(message);
                }

                Console.WriteLine($"Executing: {message.RequestUri}");
                var timer = new Stopwatch();
                timer.Start();

                var response = await _client.SendAsync(message);

                //await Task.Delay(random.Next(100, 1000));

                timer.Stop();

                var metric = new RequestData
                {
                    Elapsed = timer.ElapsedMilliseconds,
                    Endpoint = message.RequestUri.Host,
                    StatusCode = (int) response.StatusCode,
                    Url = GetFormattedUrl(message),
                    SimulatedDate = request.Created.Normalize()
                };

                if (response.IsSuccessStatusCode == false)
                {
                    request.Uri = message.RequestUri;
                    var content = await response.Content.ReadAsStringAsync();
                    var statusCode = (int) response.StatusCode;
                    var exception = new HttpRequestException($"Status code: {statusCode}. Reason: {response.ReasonPhrase}. Content: {content}");

                    SimulationTelemetry.Instance.AddException(request, exception);
                }

                SimulationTelemetry.Instance.Add(metric);
            }
            catch (Exception ex)
            {
                SimulationTelemetry.Instance.AddException(request, ex);
            }
        }

        private static string GetFormattedUrl(HttpRequestMessage message)
        {
            var url = message.RequestUri.ToString();

            return url.Length < 125 ? url : url.Substring(0, 125);
        }
    }
}
