using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using RequestSimulation.Datasources;
using RequestSimulation.Executing;
using RequestSimulation.Loadstrategies;

namespace RequestSimulation
{
    public class App
    {
        public async Task Run()
        {
            var from = DateTime.UtcNow.AddDays(-1).AddHours(7);
            var to = DateTime.UtcNow.AddDays(-1).AddHours(9);

            Console.WriteLine(" ");
            Console.WriteLine($"Start:\t {from.ToString()} (UTC)");
            Console.WriteLine($"To:\t {to.ToString()} (UTC)");
            Console.WriteLine(" ");

            var container = InitializeComponents();

            var mediator = container.Resolve<RequestMediator>();
            await mediator.PopulateRequestsAsync(from, to);

            var simulator = container.Resolve<Simulation>();
            simulator.Subscribe(mediator);
            simulator.SetLoadStrategyEffectRate(1.0);
            simulator.RunSimulation(from, to);
        }

        private IContainer InitializeComponents()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => BuildConfiguration()).As<IConfiguration>();
            builder.RegisterType<ApplicationInsightsDataSource>().As<IRequestDataSource>();
            builder.RegisterType<RequestExecutor>().As<IRequestExecutor>();
            builder.RegisterType<ExponentialLoadStrategy>().As<ILoadStrategy>();
            builder.RegisterType<ApplicationInsightsConfiguration>();
            builder.RegisterType<Simulation>();
            builder.RegisterType<RequestMediator>();

            return builder.Build();
        }

        private IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}