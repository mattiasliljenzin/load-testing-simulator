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
    public class AuthorizationInterceptorTest
    {
        private readonly IConfiguration _configuration;

        public AuthorizationInterceptorTest()
        {
            _configuration = Substitute.For<IConfiguration>();
            _configuration["Interceptors:Autho:www.google.com"].Returns("www.yahoo.com");
        }

        [Theory]
        [InlineData("http://www.google.com/api/search?q=test",   "1231231231231231231231231231231231231231231231231231231231231231")]
		[InlineData("http://www.yahoo.com/api/search?q=test",    "1111111111111111111111111111111111111111111111111111111111111111")]
		[InlineData("http://www.facebook.com/api/search?q=test", "1234567890123456789012345678901234567890123456789012345678901234")]
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