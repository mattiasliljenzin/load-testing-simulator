using System;
using System.Threading.Tasks;

namespace RequestSimulation.Executing
{
    public class ConsoleSimulationSubscriber : ISimulationSubscriber
    {
        private readonly string _name;
        public ConsoleSimulationSubscriber(string name)
        {
            _name = name;
        }

        public Task OnPublish(DateTime simulatedDate)
        {
            //Console.WriteLine($"[{_name}]Console simulator received: {simulatedDate.ToString()}");
            return Task.CompletedTask;
        }
    }
}