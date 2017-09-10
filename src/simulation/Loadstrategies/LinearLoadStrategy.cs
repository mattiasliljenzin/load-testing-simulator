using System;

namespace RequestSimulation.Loadstrategies
{
    public class LinearLoadStrategy : BaseLoadStrategy
    {
        private readonly double _slope;

        public LinearLoadStrategy(double slope = 1.5, long stepDuration = Constants.DEFAULT_STEP_DURATION_MS) : base(stepDuration)
        {
            _slope = slope;
        }

        protected override double UpdateInterval(double currentInterval)
        {
            var expInterval = Math.Exp(currentInterval / 1000);
            var interval = currentInterval / 1000;

            return (currentInterval / (expInterval / interval));
        }
    }
}