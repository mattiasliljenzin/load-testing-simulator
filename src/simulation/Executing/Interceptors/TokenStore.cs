using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace RequestSimulation.Executing.Interceptors
{
    public class TokenStore : ITokenStore
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContentClient _client;
        private readonly IDictionary<string, Func<AuthenticationHeaderValue>> _tokenFactory = new Dictionary<string, Func<AuthenticationHeaderValue>>();

        public TokenStore(IConfiguration configuration, IHttpContentClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task<IDictionary<string, Func<AuthenticationHeaderValue>>> GetAll()
        {
            if (_tokenFactory.Any())
            {
                return _tokenFactory;
            }

            try
            {
                var content = await _client.GetAsync(TokenStoreEndpoint);
                var tokens = JsonConvert.DeserializeObject<IEnumerable<TokenResult>>(content);

                foreach (var token in tokens)
                {
                    var uri = new Uri(token.Host);
                    _tokenFactory.Add(uri.Host, () => new AuthenticationHeaderValue(TokenStoreScheme, token.Token));
                }

                return _tokenFactory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred when retrieving token store: {ex.ToString()}");
                throw;
            }
        }

        private string TokenStoreEndpoint => _configuration["Interceptors:TokenStore:Endpoint"];
        private string TokenStoreScheme => _configuration["Interceptors:TokenStore:Scheme"];
    }
}
