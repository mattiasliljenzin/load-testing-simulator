using System.Timers;

namespace simulation
{
    public interface ILoadStrategy
    {
        double InitialInterval { get; }
        double GetInterval(double interval);
		void SetEffectRate(double alpha);
    }
}