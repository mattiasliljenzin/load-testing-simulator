using System;

namespace simulation.Statistics
{
    public class RequestData
    {
        public long Elapsed { get; set; }
        public string Url { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
    }
}
