using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace RequestSimulation.Executing.Interceptors
{
    public interface ITokenStore
    {
        IDictionary<string, Func<AuthenticationHeaderValue>> GetAll();
    }
}
