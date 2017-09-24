using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public class AuthorizationInterceptor : IHttpRequestMessageInterceptor
    {
        private readonly ITokenStore _store;
        private IDictionary<string, Func<AuthenticationHeaderValue>> _tokens = null;

        public AuthorizationInterceptor(ITokenStore store)
        {
            _store = store;
        }

        public async Task InterceptAsync(HttpRequestMessage message)
        {
            var host = message.RequestUri.Host.ToLower();

            if (_tokens == null)
            {
                _tokens = await _store.GetAll();
            }

            if (_tokens.ContainsKey(host))
            {
                var tokenFactory = _tokens[host];
                message.Headers.Authorization = tokenFactory();
            }
        }
    }
}
