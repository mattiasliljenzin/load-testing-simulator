using System;
using System.Collections.Generic;

namespace RequestSimulation.Executing.Interceptors
{
    public class TokenStore : ITokenStore
    {
        public IDictionary<string, string> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
