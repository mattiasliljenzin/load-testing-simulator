namespace RequestSimulation.Loadstrategies
{
    public class DoubleGrowthLoadStrategy : BaseLoadStrategy
	{
		public DoubleGrowthLoadStrategy(long stepDuration = Constants.DEFAULT_STEP_DURATION_MS) : base(stepDuration)
		{ }

		protected override double UpdateInterval(double currentInterval)
		{
            return currentInterval / 2;
		}
	}
}
