namespace RequestSimulation.Loadstrategies
{
    public class ConstantLoadStrategy : ILoadStrategy
    {
        private readonly int _multiplier;

        public ConstantLoadStrategy(int multiplier = 1)
        {
            _multiplier = multiplier;
        }

        public double InitialInterval => Constants.ONE_SECOND_IN_MS / _multiplier;

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
