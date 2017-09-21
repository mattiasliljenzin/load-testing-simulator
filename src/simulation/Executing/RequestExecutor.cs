using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using RequestSimulation.Requests;
using RequestSimulation.Statistics;

namespace RequestSimulation.Executing
{
    public class RequestExecutor : IRequestExecutor
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task Execute(ISimulatedRequest request)
        {
            try
            {
                Console.WriteLine($"Executing: {request.Uri}");

                var timer = new Stopwatch();
                timer.Start();

                
                //var response = await _client.GetAsync(request.Uri);

                timer.Stop();

                var metric = new RequestData
                {
                    Elapsed = timer.ElapsedMilliseconds,
                    Endpoint = request.Endpoint,
                    StatusCode = 200, //(int)response.StatusCode,
                    Url = request.Uri.ToString()
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
