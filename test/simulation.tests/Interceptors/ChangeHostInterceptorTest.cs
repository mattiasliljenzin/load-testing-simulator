using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using RequestSimulation.Executing.Interceptors;
using Shouldly;
using Xunit;

namespace simulation.tests.Interceptors
{
    public class ChangeHostInterceptorTest
    {
        private readonly IConfiguration _configuration = TestUtilities.BuildConfiguration();

        [Theory]
        [InlineData("http://www.google.com/api/search?q=test", "http://www.yahoo.com/api/search?q=test")]
        public void Should_intercept_as_expected(string input, string expected)
        {
            // Arrange
            var interceptor = new ChangeHostInterceptor(_configuration);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(input));

            // Act
            interceptor.InterceptAsync(message);

            // Assert
            message.RequestUri.ToString().ShouldBe(expected);
        }
    }
}