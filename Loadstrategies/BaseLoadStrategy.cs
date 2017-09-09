using System;

namespace RequestSimulation.Loadstrategies
{
    public abstract class BaseLoadStrategy : ILoadStrategy
    {
        private readonly long _stepDuration;
        private double _effectRate = 1.0;
        private DateTime _stepDurationStarted;

        protected BaseLoadStrategy(long stepDuration = Constants.DEFAULT_STEP_DURATION_MS)
        {
            _stepDuration = stepDuration;
        }

        public double InitialInterval => Constants.DEFAULT_INITIAL_INTERVAL_MS;

        public double GetInterval(double interval)
        {
            if (_stepDurationStarted == default(DateTime))
            {
                _stepDurationStarted = DateTime.UtcNow;
            }

            var now = DateTime.UtcNow;
            var elapsed = now.Subtract(_stepDurationStarted);

            if (elapsed.TotalMilliseconds > _stepDuration)
            {
                _stepDurationStarted = now;
                var currentInterval = interval;
                var newInterval = UpdateInterval(interval);
                var updatedInterval = currentInterval + ((newInterval - currentInterval) * _effectRate); 

                Console.WriteLine($"[{GetType().Name}]: Ramping up timer interval from {currentInterval} to {updatedInterval} ms (effect rate is {_effectRate * 100}%)");

                return updatedInterval;
            }
            return interval;
        }

        public void SetEffectRate(double alpha)
        {
            _effectRate = alpha;
        }

        protected abstract double UpdateInterval(double currentInterval);
    }
}
