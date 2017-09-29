using Newtonsoft.Json;

namespace RequestSimulation.Storage.ApplicationInsights
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