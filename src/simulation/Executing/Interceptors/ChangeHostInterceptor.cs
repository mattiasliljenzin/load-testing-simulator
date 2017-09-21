using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace RequestSimulation.Executing.Interceptors
{
    public class ChangeHostInterceptor : IHttpRequestMessageInterceptor
    {
        private readonly IConfiguration _configuration;

        public ChangeHostInterceptor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Intercept(HttpRequestMessage message)
        {
            var messageRequestUri = message.RequestUri;
            var host = _configuration[$"Interceptors:ChangeHost:{messageRequestUri.Host}"];

            if (string.IsNullOrWhiteSpace(host)) return;

            message.RequestUri = new Uri($"{messageRequestUri.Scheme}://{host}{messageRequestUri.PathAndQuery}");
        }
    }
}