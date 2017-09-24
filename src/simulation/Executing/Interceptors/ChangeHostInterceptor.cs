using System;
using System.Net.Http;
using System.Threading.Tasks;
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

        public Task InterceptAsync(HttpRequestMessage message)
        {
            var messageRequestUri = message.RequestUri;
            var host = _configuration[$"Interceptors:ChangeHost:{messageRequestUri.Host}"];

            if (string.IsNullOrWhiteSpace(host)) 
            { 
                return Task.CompletedTask; 
            }

            message.RequestUri = new Uri($"{messageRequestUri.Scheme}://{host}{messageRequestUri.PathAndQuery}");

            return Task.CompletedTask;
        }
    }
}