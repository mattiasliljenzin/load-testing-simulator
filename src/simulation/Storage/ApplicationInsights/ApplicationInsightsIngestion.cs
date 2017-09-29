using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RequestSimulation.Storage.ApplicationInsights
{
    public class ApplicationInsightsIngestion : IApplicationInsightsIngestion
    {
        private readonly IConfiguration _configuration;

        public ApplicationInsightsIngestion(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Trigger()
        {
            Console.WriteLine("[ApplicationInsightsIngestion]: Triggering ingestion");
            var client = new AnalyticsDataSourceClient();
            var settings = new AnalyticsDataSourceIngestionRequestSettings(_configuration);
            var request = new AnalyticsDataSourceIngestionRequest(settings);

            return await client.RequestBlobIngestion(request);
        }
    }
}