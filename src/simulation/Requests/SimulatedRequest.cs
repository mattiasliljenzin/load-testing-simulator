using System;

namespace RequestSimulation.Requests
{
    public class SimulatedRequest : ISimulatedRequest
    {
        public static ISimulatedRequest Create(string host, string path, string query, Method method, DateTime created)
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

		public static ISimulatedRequest Create(Uri uri, Method method, DateTime created)
		{
			return new SimulatedRequest
			{
				Uri = uri,
				Method = method,
				Created = created
			};
		}
        private SimulatedRequest() { }
       
        public Method Method { get; set; }
        public Uri Uri { get; set; }
        public DateTime Created { get; set; }
        public string Endpoint { get; set; }

        public override string ToString() => $"[SimulatedRequest]: {Method}: {Uri.Host}{Uri.PathAndQuery} - {Created}";
    }
}


