using System.Collections.Generic;
using System.Threading.Tasks;
using RequestSimulation.Loadstrategies;
using Xunit;

namespace simulation.tests.Loadstrategies
{
    public class LinearLoadStrategyTest
    {
        private const double BaseInterval = 1001;

        [Theory]
        [InlineData(1.0, BaseInterval)]
        public void Should_grow_linear_as_expected(double slope, double expected)
        {
            // Arrange
            var strategy = new LinearLoadStrategy(1.5D, 100);
            var currentInterval = strategy.InitialInterval;
            var samples = new List<double>();

            // Act
            for (var i = 0; i < 10; i++)
            {
                Task.Delay(75).ContinueWith(task =>
                {
                    currentInterval = strategy.GetInterval(currentInterval);
                    samples.Add(currentInterval);
                });
            }
            
            // Assert
            for (var i = 0; i < samples.Count; i++)
            {
                var sample = samples[i];

                if (i == 0)
                {
                    Assert.Equal(i, BaseInterval);
                }
                else
                {
                    
                }
            }
            Assert.Equal(expected, currentInterval);
        }
    }
}
