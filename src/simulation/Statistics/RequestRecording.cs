using System;

namespace RequestSimulation.Statistics
{
    public class RequestRecording
    {
        public long Elapsed { get; set; }
        public string Url { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
        public DateTime SimulatedDate { get; set; }
        public Guid BatchId { get; set; }
    }
}
