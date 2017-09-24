using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace RequestSimulation.Executing.Interceptors
{
    public class TokenStore : ITokenStore, INeedInitialization
    {
        private readonly IConfiguration _configuration;
        private readonly IContentClient _client;
        private static readonly IDictionary<string, Func<AuthenticationHeaderValue>> TokenFactory = new Dictionary<string, Func<AuthenticationHeaderValue>>();

        public TokenStore(IConfiguration configuration, IContentClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public IDictionary<string, Func<AuthenticationHeaderValue>> GetAll()
        {
            return TokenFactory;
        }

        public async Task Initialize()
        {
            Console.WriteLine("[INeedInitialization]: TokenStore");

            try
            {
                var content = await _client.GetAsync(TokenStoreLocation);
                var tokens = JsonConvert.DeserializeObject<IEnumerable<TokenResult>>(content);

                foreach (var token in tokens)
                {
                    var uri = new Uri(token.Host);
                    TokenFactory.Add(uri.Host, () => new AuthenticationHeaderValue(TokenStoreScheme, token.Token));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred when retrieving token store: {ex.ToString()}");
                throw;
            }
        }

        private string TokenStoreLocation => _configuration["Interceptors:TokenStore:Location"];
        private string TokenStoreScheme => _configuration["Interceptors:TokenStore:Scheme"];
    }
}
