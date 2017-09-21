using Microsoft.Extensions.Configuration;

namespace RequestSimulation.Configuration
{
    public class ApplicationInsightsConfiguration
    {
        private readonly IConfiguration _configuration;

        public ApplicationInsightsConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AppId => _configuration["AppId"];

        public string AppKey => _configuration["AppKey"];
    }
}