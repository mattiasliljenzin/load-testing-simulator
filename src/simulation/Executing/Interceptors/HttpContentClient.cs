using System.Net.Http;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public class HttpContentClient : IContentClient
    {
        public async Task<string> GetAsync(string resource)
        {
            var client = new HttpClient();
            var data = await client.GetAsync(resource);
            return await data.Content.ReadAsStringAsync();
        }
    }
}