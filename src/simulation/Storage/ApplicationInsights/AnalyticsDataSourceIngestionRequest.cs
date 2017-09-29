using System;
using Newtonsoft.Json;

namespace RequestSimulation.Storage.ApplicationInsights
{
    public class AnalyticsDataSourceIngestionRequest
    {
        #region Members 
        private const string BaseDataRequiredVersion = "2";
        private const string RequestName = "Microsoft.ApplicationInsights.OpenSchema";
        #endregion Members 

        public AnalyticsDataSourceIngestionRequest(AnalyticsDataSourceIngestionRequestSettings settings, int version = 1)
        {
            Ver = version;
            IKey = settings.Key.ToString();
            Data = new Data
            {
                BaseData = new BaseData
                {
                    Ver = BaseDataRequiredVersion,
                    BlobSasUri = settings.BlobUrlWithSas,
                    SourceName = settings.SourceId.ToString(),
                    SourceVersion = version.ToString()
                }
            };
        }

        public AnalyticsDataSourceIngestionRequest(string ikey, string schemaId, string blobSasUri, int version = 1)
        {
            Ver = version;
            IKey = ikey;
            Data = new Data
            {
                BaseData = new BaseData
                {
                    Ver = BaseDataRequiredVersion,
                    BlobSasUri = blobSasUri,
                    SourceName = schemaId,
                    SourceVersion = version.ToString()
                }
            };
        }


        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("ver")]
        public int Ver { get; set; }

        [JsonProperty("name")]
        public string Name { get { return RequestName; } }

        [JsonProperty("time")]
        public DateTime Time { get { return DateTime.UtcNow; } }

        [JsonProperty("iKey")]
        public string IKey { get; set; }
    }
}