using System;

namespace RequestSimulation.Requests
{
    public interface IMapToSimulatedRequest
    {
        DateTime TimeStamp { get; }
        string Url { get; }
    }
}