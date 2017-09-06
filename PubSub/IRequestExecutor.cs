using System;
using System.Threading.Tasks;

namespace simulation
{
    public interface IRequestExecutor
    {
        Task Execute(ISimulatedRequest request);
    }

    public class ConsoleRequestExecutor : IRequestExecutor
    {
        Task IRequestExecutor.Execute(ISimulatedRequest request)
        {
			Console.WriteLine($"Executing: {request.Uri.ToString()}");
			return Task.CompletedTask;
        }
    }
}
