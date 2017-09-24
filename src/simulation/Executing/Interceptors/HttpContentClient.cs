using System.Net.Http;
using System.Threading.Tasks;

namespace RequestSimulation.Executing.Interceptors
{
    public class HttpContentClient : IHttpContentClient
    {
        public async Task<string> GetAsync(string url)
        {
            var client = new HttpClient();
            var data = await client.GetAsync(url);
            return await data.Content.ReadAsStringAsync();
        }
    }
}