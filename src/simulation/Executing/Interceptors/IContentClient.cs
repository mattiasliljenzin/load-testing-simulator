using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public interface IContentClient
    {
        Task<string> GetAsync(string resource);
    }
}