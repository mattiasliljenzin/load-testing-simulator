using System;
using System.Web;
using Newtonsoft.Json.Linq;
using RequestSimulation.Configuration;
using RequestSimulation.Requests;

namespace RequestSimulation.Datasources
{
    public class ApplicationInsightsRequestDataSource : ApplicationInsightsDataSource
    {
        public ApplicationInsightsRequestDataSource(ApplicationInsightsConfiguration configuration) : base(configuration)
        {}

        public override string BuildQuery(DateTime from, DateTime to)
        {
            var query = $@"requests 
            | where timestamp between (datetime({from}) .. datetime({to}))
            | where name startswith ""GET ""
            | order by timestamp asc";

            //PrintQuery(query);

            return HttpUtility.UrlEncode(query);
        }

        private static void PrintQuery(string query)
        {
            Console.WriteLine(" ");
            Console.WriteLine($"[ApplicationInsightsRequestDataSource]: Query: {query}");
            Console.WriteLine(" ");
        }

        public override IMapToSimulatedRequest Map(JArray rows)
        {
            return ApplicationInsightsRequestBuilder.Create(rows);
        }
    }
}