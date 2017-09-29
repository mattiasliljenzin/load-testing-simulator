using System;

namespace simulation.core
{
    [Serializable]
    public class RequestRecording
    {
        public int Id { get; set; }
        public long Elapsed { get; set; }
        public string Url { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
        public DateTime SimulatedDate { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid BatchId { get; set; }
    }
}
