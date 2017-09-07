using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using simulation.Statistics;

namespace simulation
{
    public class Simulation
    {
        private long _counter = 0;
        private DateTime _startDate;
		private DateTime _endDate;
		private bool _simulationComplete;
		private Timer _timer;

        private readonly List<ISimulationSubscriber> _subscribers = new List<ISimulationSubscriber>();
        private readonly ILoadStrategy _loadStrategy;

        public Simulation(ILoadStrategy loadStrategy)
        {
            _loadStrategy = loadStrategy;
        }

        public void RunSimulation(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;

            _timer = new Timer(_loadStrategy.InitialInterval);
            _timer.AutoReset = true;
            _timer.Elapsed += Elapsed;
            _timer.Start();

			Console.WriteLine($"[Simulation]: Started!");
            Console.WriteLine($"[Simulation]: Load strategy: {_loadStrategy.GetType().Name}!");


            while (_simulationComplete == false){} 
        }

        private void Stop()
        {
            _timer.Stop();
            PrintReport();
        }

        private void PrintReport () 
        {
			var elapsed = DateTime.UtcNow.Subtract(_startDate);

			Console.WriteLine($"[Simulation]: Stopped!");
			Console.WriteLine($"[Simulation]: Duration was {elapsed.Seconds} seconds ({_counter + 1} simulated)");
			Console.WriteLine($"[Simulation]: Requests executed: {SimulationTelemetry.Instance.RequestCount}");
			Console.WriteLine($"[Simulation]: Request rate was: {SimulationTelemetry.Instance.RequestCount / elapsed.Seconds}/s");
			Console.WriteLine($"[Simulation]: Actual request rate was: {SimulationTelemetry.Instance.RequestCount / _counter}");
			Console.WriteLine(" ");
			Console.WriteLine(" ");
			Console.WriteLine("=== Simulation Report ===");
			SimulationTelemetry.Instance.PrintReport();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            var simulatedDate = _startDate.AddSeconds(++_counter).Normalize();

            if (simulatedDate > _endDate)
            {
				Console.WriteLine($"[Simulation]: Simulation reached end date");
                Stop();
                _simulationComplete = true;
                return;
			}

            Console.WriteLine($" ");
            Console.WriteLine($"[Simulation]: {simulatedDate}");

            Publish(simulatedDate);

            var currentInterval = _timer.Interval;
            var updatedInteval = _loadStrategy.GetInterval(currentInterval);

            if (updatedInteval > 1 && currentInterval != updatedInteval)
            {
                _timer.Interval = updatedInteval;
            }
        }

        private void Publish(DateTime dateTime)
        {
            _subscribers.ForEach(x => x.OnPublish(dateTime).ConfigureAwait(false));
        }

        public void Subscribe(ISimulationSubscriber subscriber)
        {
            Console.WriteLine($"[Simulation]: Adding subscriber: {subscriber.GetType().Name}");
            _subscribers.Add(subscriber);
        }
	}
}
