using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using RequestSimulation.Extensions;
using RequestSimulation.Requests;
using RequestSimulation.Statistics;

namespace RequestSimulation.Executing
{
    public class RequestExecutor : IRequestExecutor
    {
        private readonly HttpClient _client = new HttpClient();
        private Random random = new Random();

        public async Task Execute(ISimulatedRequest request)
        {
            try
            {
                Console.WriteLine($"Executing: {request.Uri}");

                var timer = new Stopwatch();
                timer.Start();

                await Task.Delay(random.Next(10, (DateTime.Now.Second * 10)));
                //var response = await _client.GetAsync(request.Uri);

                timer.Stop();

                var metric = new RequestData
                {
                    Elapsed = timer.ElapsedMilliseconds,
                    Endpoint = request.Endpoint,
                    StatusCode = 200, //(int)response.StatusCode,
                    Url = request.Uri.ToString(),
                    Created = request.Created,
                    Timestamp = DateTime.UtcNow.Normalize()
                };

                SimulationTelemetry.Instance.Add(metric);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                SimulationTelemetry.Instance.AddException(request, ex);
            }
        }
    }
}
