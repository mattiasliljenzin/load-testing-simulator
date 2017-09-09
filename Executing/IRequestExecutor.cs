using System.Threading.Tasks;
using RequestSimulation.Requests;

namespace RequestSimulation.Executing
{
    public interface IRequestExecutor
    {
        Task Execute(ISimulatedRequest request);
    }
}
