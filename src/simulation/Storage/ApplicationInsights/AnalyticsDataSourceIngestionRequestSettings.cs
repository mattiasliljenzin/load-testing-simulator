using System;
using Microsoft.Extensions.Configuration;

namespace RequestSimulation.Storage.ApplicationInsights
{
    public class AnalyticsDataSourceIngestionRequestSettings
    {
        private readonly IConfiguration _configuration;

        public AnalyticsDataSourceIngestionRequestSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Guid Key => Guid.Parse(GetSetting("InstrumentationKey"));
        public Guid SourceId => Guid.Parse(GetSetting("SourceId"));
        public string BlobUrlWithSas => GetSetting("BlobUrlWithSas");

        private string GetSetting(string key)
        {
            return _configuration[$"Packages:ApplicationInsightsResultStorage:Ingestion:{key}"];
        }
    }
}