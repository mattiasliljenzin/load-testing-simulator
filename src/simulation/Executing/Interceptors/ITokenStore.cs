using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public interface ITokenStore
    {
        Task<IDictionary<string, Func<AuthenticationHeaderValue>>> GetAll();
    }
}
