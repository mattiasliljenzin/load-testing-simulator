using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequestSimulation.Loadstrategies;
using Xunit;

namespace simulation.tests.Loadstrategies
{
    public class ConstantLoadStrategyTest
    {
        [Fact]
        public async Task Should_grow_as_expected()
        {
            // Arrange
            var strategy = new ConstantLoadStrategy();
            var currentInterval = strategy.InitialInterval;
            var samples = new List<double>();

            // Act
            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(125);
                currentInterval = strategy.GetInterval(currentInterval);
                samples.Add(currentInterval);
            }

            // Assert
            for (var i = 0; i < samples.Count; i = i + 2)
            {
                var s1 = samples[i];
                var s2 = samples[i + 1];

                Console.WriteLine($"{s1} - {s2}");

                Assert.True(Math.Abs(s1 - s2) < 0.1);
            }
        }
    }
}