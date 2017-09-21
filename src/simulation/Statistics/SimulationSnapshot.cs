using System;
using System.Collections.Generic;

namespace RequestSimulation.Statistics
{
    public class SimulationSnapshot
    {
        public double SimulatedSpeedMultiplier { get; set; }
        public double Progress { get; set; }
        public DateTime Timestamp { get; set; }
        public List<RequestData> Requests { get; set; }
    }
}