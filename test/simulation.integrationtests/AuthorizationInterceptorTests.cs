using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Core;
using RequestSimulation.Executing.Interceptors;
using Shouldly;
using Xunit;

namespace simulation.integrationtests
{
    public class AuthorizationInterceptorTests
    {
        private const string GoogleToken = "1231231231231231231231231231231231231231231231231231231231231231";
        private const string YahooToken = "1111111111111111111111111111111111111111111111111111111111111111";
        private const string FacebookToken = "1234567890123456789012345678901234567890123456789012345678901234";
        private readonly IConfiguration _configuration = TestUtilities.BuildConfiguration();

        [Fact]
        public async Task Should_retrieve_from_token_store()
        {
            // Arrange
            var client = CreateMockedClientAsync();
            var store = new TokenStore(_configuration, client);

            // Act
            var tokens = await store.GetAll();

            // Assert
            tokens.Count.ShouldBe(3);
            tokens["www.google.com"]().Parameter.ShouldBe(GoogleToken);
            tokens["www.yahoo.com"]().Parameter.ShouldBe(YahooToken);
            tokens["www.facebook.com"]().Parameter.ShouldBe(FacebookToken);

        }

        private IHttpContentClient CreateMockedClientAsync()
        {
            var client = Substitute.For<IHttpContentClient>();
            client.GetAsync(null).ReturnsForAnyArgs(CreateFakeTokenResultTask);

            return client;
        }

        private async Task<string> CreateFakeTokenResultTask(CallInfo callInfo)
        {
            var json = JsonConvert.SerializeObject(CreateFakeTokenResult());
            return await Task.FromResult(json);
        }

        private IEnumerable<TokenResult> CreateFakeTokenResult()
        {
            yield return new TokenResult
            {
                Host = "https://www.google.com",
                Token = GoogleToken
            };

            yield return new TokenResult
            {
                Host = "https://www.yahoo.com",
                Token = YahooToken
            };

            yield return new TokenResult
            {
                Host = "https://www.facebook.com",
                Token = FacebookToken
            };
        }
    }
}