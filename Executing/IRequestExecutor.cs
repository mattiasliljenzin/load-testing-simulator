using System.Threading.Tasks;

namespace simulation
{
    public interface IRequestExecutor
    {
        Task Execute(ISimulatedRequest request);
    }
}
