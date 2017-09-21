using System;
using System.Collections.Generic;
using System.Timers;
using RequestSimulation.Executing;
using RequestSimulation.Extensions;
using RequestSimulation.Loadstrategies;
using RequestSimulation.Statistics;

namespace RequestSimulation
{
    public class Simulation
    {
        private long _counter;
        private DateTime _simulatedStartDate;
		private DateTime _simulatedEndDate;
        private DateTime _simulationStartedDate;
		private bool _simulationComplete;
		private Timer _timer;

        private readonly List<ISimulationSubscriber> _subscribers = new List<ISimulationSubscriber>();
        private readonly ILoadStrategy _loadStrategy;

        public Simulation(ILoadStrategy loadStrategy)
        {
            _loadStrategy = loadStrategy;
        }

        public void RunSimulation(DateTime simulatedStartDate, DateTime simulatedEndDate)
        {
            _simulatedStartDate = simulatedStartDate;
            _simulatedEndDate = simulatedEndDate;
            _simulationStartedDate = DateTime.UtcNow;

            _timer = new Timer(_loadStrategy.InitialInterval) {AutoReset = true};
            _timer.Elapsed += Elapsed;
            _timer.Start();

			Console.WriteLine($"[Simulation]: Started!");
            Console.WriteLine($"[Simulation]: Load strategy: {_loadStrategy.GetType().Name}!");


            while (_simulationComplete == false)
            {
                // wait for it..
            } 
        }

        public void SetLoadStrategyEffectRate(double alpha)
        {
            _loadStrategy.SetEffectRate(alpha);
        }

        private void Stop()
        {
            _timer.Stop();
            PrintReport();
        }

        private void PrintReport () 
        {
			var elapsed = DateTime.UtcNow.Subtract(_simulationStartedDate);
			Console.WriteLine($"[Simulation]: Stopped!");
			Console.WriteLine($"[Simulation]: Duration was {Math.Ceiling(elapsed.TotalSeconds)} seconds ({_counter} simulated)");
			Console.WriteLine($"[Simulation]: Average simulation speed was {Math.Ceiling(_counter / elapsed.TotalSeconds)}X");
            Console.WriteLine($"[Simulation]: Requests executed: {SimulationTelemetry.Instance.RequestCount}");
			Console.WriteLine($"[Simulation]: Request rate was: {SimulationTelemetry.Instance.RequestCount / elapsed.Seconds}/s");
			Console.WriteLine($"[Simulation]: Actual request rate was: {SimulationTelemetry.Instance.RequestCount / _counter}");
			Console.WriteLine(" ");
			Console.WriteLine(" ");
			Console.WriteLine("=== Simulation Report ===");
            Console.WriteLine(" ");
			SimulationTelemetry.Instance.PrintReport();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            var simulatedDate = _simulatedStartDate.AddSeconds(++_counter).Normalize();

            if (simulatedDate > _simulatedEndDate)
            {
                Stop();
                SetCompleted();
                return;
			}

            //LogTick(simulatedDate);
            LogProgress(simulatedDate);
            Publish(simulatedDate);

            var currentInterval = _timer.Interval;
            var updatedInteval = _loadStrategy.GetInterval(currentInterval);

            if (ThresholdReached(updatedInteval, currentInterval) == false)
            {
                _timer.Interval = updatedInteval;
            }
        }

        private void LogProgress(DateTime simulatedDate)
        {
            var elapsed = simulatedDate.Subtract(_simulatedStartDate);
            var total = _simulatedEndDate.Subtract(_simulatedStartDate);
            var progress = (elapsed / total);

            Console.WriteLine(" ");
            Console.WriteLine($"PROGRESS: {progress:P}%");
            Console.WriteLine(" ");

            SimulationTelemetry.Instance.Add(new SimulationSnapshot
            {
                SimulatedSpeedMultiplier = Constants.ONE_SECOND_IN_MS / _timer.Interval,
                Progress = progress,
                Timestamp = simulatedDate
            });
        }

        private static void LogTick(DateTime simulatedDate)
        {
            Console.WriteLine($" ");
            Console.WriteLine($"[Simulation]: {simulatedDate}");
        }

        private static bool ThresholdReached(double updatedInteval, double currentInterval)
        {
            const int thresholdMilliseconds = 10;

            if (updatedInteval <= thresholdMilliseconds) return true;

            return Math.Abs(currentInterval - updatedInteval) <= thresholdMilliseconds;
        }

        private void SetCompleted()
        {
            Console.WriteLine($"[Simulation]: Simulation reached end date");
            _simulationComplete = true;
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
