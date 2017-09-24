using System.IO;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public class FileContentClient : IContentClient
    {
        public async Task<string> GetAsync(string resource)
        {
            if (File.Exists(resource))
            {
                return await File.ReadAllTextAsync(resource);
            }
            throw new FileNotFoundException($"Could not find any file at {resource}. Throwing exception...");
        }
    }
}