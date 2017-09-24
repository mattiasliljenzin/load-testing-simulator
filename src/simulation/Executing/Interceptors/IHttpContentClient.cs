using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public interface IHttpContentClient
    {
        Task<string> GetAsync(string url);
    }
}