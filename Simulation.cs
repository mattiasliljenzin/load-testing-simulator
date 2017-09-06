using System;
using System.Collections.Generic;
using System.Timers;
using simulation.Statistics;

namespace simulation
{
    public class Simulation
    {
        private long _counter = 0;
        private DateTime _startDate;
        private Timer _timer;

        private readonly List<ISimulationSubscriber> _subscribers = new List<ISimulationSubscriber>();
        private readonly ILoadStrategy _loadStrategy;

        public Simulation(ILoadStrategy loadStrategy)
        {
            _loadStrategy = loadStrategy;
        }

        public void Start(DateTime startDate)
        {
            _startDate = startDate;

            _timer = new Timer(_loadStrategy.InitialInterval);
            _timer.AutoReset = true;
            _timer.Elapsed += Elapsed;
            _timer.Start();

            Console.WriteLine("[Simulation]: Started!");
        }

        public void Stop()
        {
            _timer.Stop();
            var elapsed = DateTime.UtcNow.Subtract(_startDate);

            Console.WriteLine($"[Simulation]: Stopped!");
            Console.WriteLine($"[Simulation]: Duration was {elapsed.Seconds} seconds ({_counter + 1} simulated)");
            Console.WriteLine($"[Simulation]: Requests executed: {SimulationTelemetry.Instance.RequestCount}");
			Console.WriteLine(" ");
			Console.WriteLine(" ");
			Console.WriteLine("=== Simulation Report ===");
            SimulationTelemetry.Instance.PrintReport();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            var simulatedDate = _startDate.AddSeconds(++_counter).Normalize();

            Console.WriteLine($" ");
            Console.WriteLine($"[Simulation]: {simulatedDate}");

            Publish(simulatedDate);

            var currentInterval = _timer.Interval;
            var updatedInteval = _loadStrategy.GetInterval(currentInterval);

            if (currentInterval != updatedInteval)
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
