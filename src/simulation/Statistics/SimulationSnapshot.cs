using System;
using System.Collections.Generic;

namespace RequestSimulation.Statistics
{
    public class SimulationSnapshot
    {
        public double SimulatedSpeedMultiplier { get; set; }
        public double Progress { get; set; }
        public DateTime SimulatedDate { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<RequestRecording> Requests { get; set; }
    }
}