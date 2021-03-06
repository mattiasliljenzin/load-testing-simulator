using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RequestSimulation.Configuration;
using RequestSimulation.Datasources;
using RequestSimulation.Requests;
using Shouldly;
using Xunit;

namespace simulation.integrationtests
{
    public class ApplicationInsightsDataSourceTests
    {
        private readonly IConfiguration _configuration = TestUtilities.BuildConfiguration();

        [Fact]
        public async Task Should_map_result_When_retrieving_requests()
        {
            // Arrange
            var source = new ApplicationInsightsRequestDataSource(CreateConfiguration());

            // Act
            var result = await source.GetAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            AssertResult(result);
        }

        [Fact] // This will fail with DEMO_APP since there is no host in that data, but work with your data
        public async Task Should_map_result_When_retrieving_dependencies()
        {
            // Arrange
            var source = new ApplicationInsightsDependencyDataSource(CreateConfiguration());

            // Act
            var result = await source.GetAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

            // Assert
            AssertResult(result);
        }

        private static void AssertResult(IDictionary<DateTime, IList<ISimulatedRequest>> result)
        {
            result.ShouldNotBeNull();
            result.Keys.ShouldNotBeEmpty();
            result.Values.ShouldNotBeEmpty();
            result.Values.All(x => x.Any()).ShouldBeTrue();

            foreach (var date in result.Keys)
            {
                foreach (var request in result[date])
                {
                    request.Created.ShouldNotBe(default(DateTime));
                    request.Endpoint.ShouldBe(null);
                    request.Uri.ShouldNotBeNull();
                }
            }
        }

        private ApplicationInsightsConfiguration CreateConfiguration()
        {
            return new ApplicationInsightsConfiguration(_configuration);
        }
    }
}
