using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RequestSimulation.Executing.Interceptors;
using Shouldly;
using Xunit;

namespace simulation.integrationtests
{
    public class AuthorizationInterceptorTests
    {
        private readonly IConfiguration _configuration = TestUtilities.BuildConfiguration();

        [Fact]
        public async Task Should_retrieve_from_token_store()
        {
            // Arrange
            var expected = 15; // set this to corresponding test data
            var client = new FileContentClient();
            var store = new TokenStore(_configuration, client);
            await store.Initialize();

            // Act
            var tokens = store.GetAll();

            // Assert
            tokens.Count.ShouldBe(expected);
            tokens.All(x => string.IsNullOrWhiteSpace(x.Value().Scheme) == false).ShouldBeTrue();
            tokens.All(x => string.IsNullOrWhiteSpace(x.Value().Parameter) == false).ShouldBeTrue();
        }
    }
}