using System.Threading.Tasks;

namespace RequestSimulation
{
    public interface INeedInitialization
    {
        Task Initialize();
    }
}