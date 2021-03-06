using System;

namespace RequestSimulation.Requests
{
    public class SimulatedRequest : ISimulatedRequest
    {
        public static ISimulatedRequest Create(string host, string path, string query, string method, DateTime created)
        {
            var builder = new UriBuilder
            {
                Host = host,
                Path = path,
                Query = query
            };
            return new SimulatedRequest
            {
                Method = method,
                Created = created,
                Uri = builder.Uri
            };
        }

        public static ISimulatedRequest Create(IMapToSimulatedRequest request, string method = "GET")
        {
            try
            {
                return new SimulatedRequest
                {
                    Uri = new Uri(request.Url),
                    Method = method,
                    Created = request.TimeStamp
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private SimulatedRequest() { }
       
        public string Method { get; set; }
        public Uri Uri { get; set; }
        public DateTime Created { get; set; }
        public string Endpoint { get; set; }

        public override string ToString() => $"[SimulatedRequest]: {Method}: {Uri.Host}{Uri.PathAndQuery} - {Created}";
    }
}


