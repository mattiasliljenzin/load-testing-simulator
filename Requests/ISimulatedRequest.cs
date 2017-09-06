using System;

namespace simulation
{
    public interface ISimulatedRequest
    {
        Method Method { get; set; }
        DateTime Created { get; set; }
        Uri Uri { get; set; }
    }
}


