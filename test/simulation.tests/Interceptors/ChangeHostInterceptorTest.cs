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
        private readonly IConfiguration _configuration;

        public ChangeHostInterceptorTest()
        {
            _configuration = Substitute.For<IConfiguration>();
            _configuration["Interceptors:ChangeHost:www.google.com"].Returns("www.yahoo.com");
        }

        [Theory]
        [InlineData("http://www.google.com/api/search?q=test", "http://www.yahoo.com/api/search?q=test")]
        public void Should_intercept_as_expected(string input, string expected)
        {
            // Arrange
            var interceptor = new ChangeHostInterceptor(_configuration);
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(input));

            // Act
            interceptor.Intercept(message);

            // Assert
            message.RequestUri.ToString().ShouldBe(expected);
        }
    }
}