using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public class AuthorizationInterceptor : IHttpRequestMessageInterceptor
    {
        private readonly ITokenStore _store;

        public AuthorizationInterceptor(ITokenStore store)
        {
            _store = store;
        }

        public void InterceptAsync(HttpRequestMessage message)
        {
            var host = message.RequestUri.Host.ToLower();
            var tokens = _store.GetAll();

            if (tokens.ContainsKey(host))
            {
                var tokenFactory = tokens[host];
                message.Headers.Authorization = tokenFactory();
            }
        }
    }
}
