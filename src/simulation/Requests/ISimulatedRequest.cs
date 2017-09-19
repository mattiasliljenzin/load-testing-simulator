using System;

namespace RequestSimulation.Requests
{
    public interface ISimulatedRequest
    {
        DateTime Created { get; set; }
        Uri Uri { get; set; }
		string Endpoint { get; set; }
	}
}


