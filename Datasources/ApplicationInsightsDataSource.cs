using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace simulation
{
    public class ApplicationInsightsDataSource : IRequestDataSource
    {
        private readonly ApplicationInsightsConfiguration _configuration;

        public ApplicationInsightsDataSource(ApplicationInsightsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IDictionary<DateTime, IList<ISimulatedRequest>>> GetAsync(DateTime from, DateTime to)
        {
            var appId = _configuration.AppId;
            var appKey = _configuration.AppKey;
            var query = BuildQuery(from, to);

            var builder = new UriBuilder()
            {
                Scheme = Uri.UriSchemeHttps,
                Host = "api.applicationinsights.io",
                Path = $"beta/apps/{appId}/query",
                Query = $"query={query}"
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", appKey);

            System.Console.WriteLine($"[ApplicationInsightsRequestSourceService]: Building request '{builder.ToString()}'");
            System.Console.WriteLine($"[ApplicationInsightsRequestSourceService]: Querying items...");
            
            var content = await client.GetStringAsync(builder.ToString());
            
            //File.AppendAllText("ai-dump.txt", content);

            var rows = (JArray)JObject.Parse(content)["Tables"][0]["Rows"];
            var requests = rows.Select(x => ApplicationInsightsRequestBuilder.Create((JArray)x)).ToArray();

			System.Console.WriteLine($"[ApplicationInsightsRequestSourceService]: {content.Length} bytes received");
            System.Console.WriteLine($"[ApplicationInsightsRequestSourceService]: {requests.Length} requests generated");

			var result = new Dictionary<DateTime, IList<ISimulatedRequest>>();

            foreach (var request in requests)
            {
                var timestamp = request.timestamp.Normalize();
                if(!result.ContainsKey(timestamp)) {
                    result[timestamp] = new List<ISimulatedRequest>();
                }

                var simulatedRequest = SimulatedRequest.Create(new Uri(request.url), Method.GET, timestamp);
                simulatedRequest.Endpoint = request.operation_Name;

                result[timestamp].Add(simulatedRequest);
            }

            return result;
        }

        private string BuildQuery(DateTime from, DateTime to)
        {
            var query = $@"requests 
            | where timestamp between (datetime({from}) .. datetime({to}))
            | where name startswith ""GET ""
            | order by timestamp asc";

            return System.Web.HttpUtility.UrlEncode(query);
        }
    }

    public class ApplicationInsightsConfiguration
    {
        private readonly string _appId;
        private readonly string _appKey;

        public ApplicationInsightsConfiguration(string appId, string appKey)
        {
            _appKey = appKey;
            _appId = appId;
        }

        public string AppId => _appId;

        public string AppKey => _appKey;
    }
}
