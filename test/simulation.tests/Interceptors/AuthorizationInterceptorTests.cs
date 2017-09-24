using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.Core;
using RequestSimulation.Executing.Interceptors;
using Shouldly;
using Xunit;

namespace simulation.tests.Interceptors
{
    public class AuthorizationInterceptorTest
    {
        [Theory]
        [InlineData("http://www.google.com/api/search?q=test",   "Bearer 1231231231231231231231231231231231231231231231231231231231231231")]
		[InlineData("http://www.yahoo.com/api/search?q=test", "Bearer 1111111111111111111111111111111111111111111111111111111111111111")]
		[InlineData("http://www.facebook.com/api/search?q=test", "Bearer 1234567890123456789012345678901234567890123456789012345678901234")]
        public async Task Should_intercept_as_expected(string input, string expected)
        {
            // Arrange
            var store = CreateMockedTokenStore();
            var interceptor = new AuthorizationInterceptor(store);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(input));

            // Act
            await interceptor.InterceptAsync(message);

            // Assert
            message.Headers.Authorization.Scheme.ShouldBe(expected.Split(' ')[0]);
            message.Headers.Authorization.Parameter.ShouldBe(expected.Split(' ')[1]);
        }

        private ITokenStore CreateMockedTokenStore()
        {
            var store = Substitute.For<ITokenStore>();
            var data = new Dictionary<string, Func<AuthenticationHeaderValue>>
            {
                {
                    "www.google.com",
                    () => new AuthenticationHeaderValue("Bearer", "1231231231231231231231231231231231231231231231231231231231231231")
                },
                {
                    "www.yahoo.com",
                    () => new AuthenticationHeaderValue("Bearer", "1111111111111111111111111111111111111111111111111111111111111111")
                },
                {
                    "www.facebook.com",
                    () => new AuthenticationHeaderValue("Bearer", "1234567890123456789012345678901234567890123456789012345678901234")
                }
            };

            store.GetAll().Returns(data);

            return store;
        }
    }
}