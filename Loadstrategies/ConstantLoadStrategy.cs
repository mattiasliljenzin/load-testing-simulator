using System;
using System.Timers;

namespace simulation
{
    public class ConstantLoadStrategy : ILoadStrategy
    {
        private readonly int _multiplier;

        public ConstantLoadStrategy(int multiplier)
        {
            _multiplier = multiplier;
        }

        public double InitialInterval => Constants.DEFAULT_INITIAL_INTERVAL_MS / _multiplier;

        public double GetInterval(double interval)
        {
            return interval;
        }

        public void SetEffectRate(double alpha)
        {
            // constant
        }
    }
}
