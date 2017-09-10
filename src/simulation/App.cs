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
            var from = DateTime.UtcNow.AddHours(-1);
            var to = DateTime.UtcNow;

            var container = InitializeComponents();

            var mediator = container.Resolve<RequestMediator>();
            await mediator.PopulateRequestsAsync(from, to);

            var simulator = container.Resolve<Simulation>();
            simulator.Subscribe(mediator);
            simulator.SetLoadStrategyEffectRate(0.75);
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