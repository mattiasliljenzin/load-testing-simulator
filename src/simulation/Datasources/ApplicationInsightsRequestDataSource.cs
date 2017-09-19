using System;
using System.Web;
using Newtonsoft.Json.Linq;
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

            return HttpUtility.UrlEncode(query);
        }

        public override IMapToSimulatedRequest Map(JArray rows)
        {
            return ApplicationInsightsRequestBuilder.Create(rows);
        }
    }
}