using System;
using System.Web;
using Newtonsoft.Json.Linq;
using RequestSimulation.Configuration;
using RequestSimulation.Requests;

namespace RequestSimulation.Datasources
{
    public class ApplicationInsightsDependencyDataSource : ApplicationInsightsDataSource
    {
        public ApplicationInsightsDependencyDataSource(ApplicationInsightsConfiguration configuration) : base(configuration)
        { }

        public override string BuildQuery(DateTime from, DateTime to)
        {
            var query = $@"dependencies 
            | where timestamp between (datetime({from}) .. datetime({to}))
            | where name startswith ""GET ""
            | order by timestamp asc";

            return HttpUtility.UrlEncode(query);
        }

        public override IMapToSimulatedRequest Map(JArray rows)
        {
            return ApplicationInsightsDependencyBuilder.Create(rows);
        }
    }
}