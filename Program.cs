using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace simulation
{
    public class Program
    {
        static void Main(string[] args)
        {
			var from = DateTime.UtcNow.AddHours(-1);
			var to = DateTime.UtcNow;

			var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("config.json")
				.AddEnvironmentVariables()
				.Build();

            var appId = configuration["appId"];
            var appKey = configuration["appKey"];
            var settings = new ApplicationInsightsConfiguration(appId, appKey);
            
            var requestDataSource = new ApplicationInsightsDataSource(settings);
            var requestExecutor = new RequestExecutor();
            var requestScheduler = new RequestScheduler(requestDataSource, requestExecutor);
            //var test = new ConsoleSimulationSubscriber("test-run");

            var populateTask = requestScheduler.PopulateRequestsAsync(from, to);
            populateTask.Wait();

            var constantStrategy = new ConstantLoadStrategy(1);
            var exponentialStrategy = new ExponentialLoadStrategy();
            var doubleGrowthStrategy = new DoubleGrowthLoadStrategy();

            var strategy = exponentialStrategy;
            strategy.SetEffectRate(0.75);

            var simulation = new Simulation(strategy);
            simulation.Subscribe(requestScheduler);

            simulation.RunSimulation(from, to);

            System.Console.WriteLine(" ");
            System.Console.WriteLine("Good-bye world!");
        }
    }

    
}
