using System;
using System.Threading.Tasks;

namespace simulation
{
    public interface ISimulationSubscriber
    {
        Task OnPublish(DateTime simulatedDate);
    }
}