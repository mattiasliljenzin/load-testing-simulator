using System.Threading.Tasks;

namespace RequestSimulation.Storage.ApplicationInsights
{
    public interface IApplicationInsightsIngestion
    {
        Task<bool> Trigger();
    }
}