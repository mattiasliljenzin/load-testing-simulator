using System.Net.Http;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public interface IHttpRequestMessageInterceptor
    {
        Task InterceptAsync(HttpRequestMessage message);
    }
}