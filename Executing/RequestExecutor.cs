using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using simulation.Statistics;

namespace simulation
{
    public class RequestExecutor : IRequestExecutor
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task Execute(ISimulatedRequest request)
        {
			Console.WriteLine($"Executing: {request.Uri.ToString()}");

			var timer = new Stopwatch();
			timer.Start();

			var response = await _client.GetAsync(request.Uri);

			timer.Stop();

			var metric = new RequestData()
			{
				Elapsed = timer.ElapsedMilliseconds,
				Endpoint = request.Endpoint,
				StatusCode = (int)response.StatusCode,
				Url = request.Uri.ToString()
			};

            SimulationTelemetry.Instance.Add(metric);
        }
    }
}
