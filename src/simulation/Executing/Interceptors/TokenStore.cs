using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public class TokenStore : ITokenStore
    {
        public Task<IDictionary<string, Func<AuthenticationHeaderValue>>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
