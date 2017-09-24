using System.IO;
using Microsoft.Extensions.Configuration;

namespace simulation.tests
{
    public static class TestUtilities
    {
        public static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.example.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}