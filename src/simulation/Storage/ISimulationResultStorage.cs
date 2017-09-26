using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RequestSimulation.Statistics;

namespace RequestSimulation.Storage
{
    public interface ISimulationResultStorage
    {
        Task Save(ICollection<RequestRecording> request);
    }
}
