using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RequestSimulation.Extensions;
using RequestSimulation.Requests;

namespace RequestSimulation.Datasources
{
    public abstract class ApplicationInsightsDataSource : IRequestDataSource
    {
        private readonly ApplicationInsightsConfiguration _configuration;

        protected ApplicationInsightsDataSource(ApplicationInsightsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IDictionary<DateTime, IList<ISimulatedRequest>>> GetAsync(DateTime from, DateTime to)
        {
            var appId = _configuration.AppId;
            var appKey = _configuration.AppKey;
            var query = BuildQuery(from, to);
            var builder = CreateBuilder(appId, query);
            var client = CreateHttpClient(appKey);

            LogPreProgress(builder);

            var content = await client.GetStringAsync(builder.ToString());
            var requests = MapResult(content);

            LogPostProgress(content, requests);

            return CreateResult(requests);
        }

        private static IDictionary<DateTime, IList<ISimulatedRequest>> CreateResult(IMapToSimulatedRequest[] requests)
        {
            var result = new Dictionary<DateTime, IList<ISimulatedRequest>>();

            foreach (var request in requests)
            {
                var timestamp = request.TimeStamp.Normalize();
                if (!result.ContainsKey(timestamp))
                {
                    result[timestamp] = new List<ISimulatedRequest>();
                }

                result[timestamp].Add(SimulatedRequest.Create(request));
            }

            return result;
        }

        private static void LogPostProgress(string content, IMapToSimulatedRequest[] requests)
        {
            Console.WriteLine($"[ApplicationInsightsRequestSourceService]: {content.Length} bytes received");
            Console.WriteLine($"[ApplicationInsightsRequestSourceService]: {requests.Length} requests generated");
        }

        private IMapToSimulatedRequest[] MapResult(string content)
        {
            //System.IO.File.AppendAllText("ai-dependency-dump.txt", content);

            var rows = (JArray) JObject.Parse(content)["Tables"][0]["Rows"];
            return rows.Select(x => Map((JArray) x)).ToArray();
        }

        private static void LogPreProgress(UriBuilder builder)
        {
            Console.WriteLine($"[ApplicationInsightsRequestSourceService]: Building request '{builder}'");
            Console.WriteLine($"[ApplicationInsightsRequestSourceService]: Querying items...");
        }

        private static HttpClient CreateHttpClient(string appKey)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", appKey);
            return client;
        }

        private static UriBuilder CreateBuilder(string appId, string query)
        {
            return new UriBuilder
            {
                Scheme = Uri.UriSchemeHttps,
                Host = "api.applicationinsights.io",
                Path = $"beta/apps/{appId}/query",
                Query = $"query={query}"
            };
        }

        public abstract string BuildQuery(DateTime from, DateTime to);
        public abstract IMapToSimulatedRequest Map(JArray rows);
    }
}
