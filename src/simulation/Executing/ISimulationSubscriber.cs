using System;
using System.Threading.Tasks;

namespace RequestSimulation.Executing
{
    public interface ISimulationSubscriber
    {
        Task OnPublish(DateTime simulatedDate);
    }
}