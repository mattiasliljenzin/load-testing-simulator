using System;

namespace RequestSimulation.Loadstrategies
{

    public class ExponentialLoadStrategy : BaseLoadStrategy
    {
        public ExponentialLoadStrategy(long stepDuration = Constants.DEFAULT_STEP_DURATION_MS) : base(stepDuration)
        {}

        protected override double UpdateInterval(double currentInterval)
        {
			var expInterval = Math.Exp(currentInterval / 1000);
			var interval = currentInterval / 1000;

            return (currentInterval / (expInterval / interval));
		}
    }
}
