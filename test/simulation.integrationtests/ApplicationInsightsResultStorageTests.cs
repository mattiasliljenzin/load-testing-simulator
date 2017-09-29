using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using RequestSimulation.Statistics;
using RequestSimulation.Storage.ApplicationInsights;
using Shouldly;
using Xunit;

namespace simulation.integrationtests
{
    public class ApplicationInsightsResultStorageTests
    {
        private readonly IConfiguration _configuration = TestUtilities.BuildConfiguration();

        [Fact]
        public async Task Can_upload_data_to_blog_storage()
        {
            // Arrange
            var data = await GetDataFromSql();
            var storage = new ApplicationInsightsResultStorage(_configuration);

            // Act
            await storage.Save(data.ToList());

            // Assert

        }

        [Fact]
        public async Task Can_trigger_data_source_ingestion()
        {
            // Arrange
            var client = new AnalyticsDataSourceClient();
            var settings = new AnalyticsDataSourceIngestionRequestSettings(_configuration);
            var ingestionRequest = new AnalyticsDataSourceIngestionRequest(settings);

            // Act
            var success = await client.RequestBlobIngestion(ingestionRequest);

            // Assert
            success.ShouldBeTrue();

        }

        private async Task<IEnumerable<RequestRecording>> GetDataFromSql()
        {
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:SQL"]))
            {
                return await connection.QueryAsync<RequestRecording>(new CommandDefinition("SELECT * FROM [dbo].[RequestRecordings] WHERE Timestamp IS NOT NULL ORDER BY ID desc"));
            }
        }
    }
}