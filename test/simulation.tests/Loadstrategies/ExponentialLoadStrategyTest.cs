using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequestSimulation.Loadstrategies;
using Xunit;

namespace simulation.tests.Loadstrategies
{
    public class ExponentialLoadStrategyTest
    {
        [Fact]
        public async Task Should_grow_as_expected()
        {
            // Arrange
            var strategy = new ExponentialLoadStrategy(100);
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

                var expInterval = Math.Exp(s1 / 1000);
                var interval = s1 / 1000;
                var newInterval = s1 / (expInterval / interval);

                Assert.True(Math.Abs(s2 - newInterval) < 0.1);
            }
        }
    }
}