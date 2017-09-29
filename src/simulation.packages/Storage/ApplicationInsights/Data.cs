using Newtonsoft.Json;

namespace simulation.packages.Storage.ApplicationInsights
{
    public class Data
    {
        private const string DataBaseType = "OpenSchemaData";

        [JsonProperty("baseType")]
        public string BaseType => DataBaseType;

        [JsonProperty("baseData")]
        public BaseData BaseData { get; set; }
    }
}