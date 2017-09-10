namespace RequestSimulation.Loadstrategies
{
    public interface ILoadStrategy
    {
        double InitialInterval { get; }
        double GetInterval(double interval);
		void SetEffectRate(double alpha);
    }
}