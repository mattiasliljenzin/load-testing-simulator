using Newtonsoft.Json;

namespace simulation.packages.Storage.ApplicationInsights
{
    public class BaseData
    {
        [JsonProperty("ver")]
        public string Ver { get; set; }

        [JsonProperty("blobSasUri")]
        public string BlobSasUri { get; set; }

        [JsonProperty("sourceName")]
        public string SourceName { get; set; }

        [JsonProperty("sourceVersion")]
        public string SourceVersion { get; set; }
    }
}