using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using RequestSimulation.Configuration;
using RequestSimulation.Datasources;
using RequestSimulation.Executing;
using RequestSimulation.Executing.Interceptors;
using RequestSimulation.Loadstrategies;
using RequestSimulation.Statistics;
using RequestSimulation.Storage;
using RequestSimulation.Storage.ApplicationInsights;
using RequestSimulation.Storage.Sql;

namespace RequestSimulation
{
    public class App
    {
        public async Task Run()
        {
            var from = new DateTime(2017, 09, 28, 13, 00, 00, DateTimeKind.Utc);
            var to = from.AddMinutes(60);

            Console.WriteLine(" ");
            Console.WriteLine($"Start:\t {from.ToString()} (UTC)");
            Console.WriteLine($"To:\t {to.ToString()} (UTC)");
            Console.WriteLine(" ");

            var container = InitializeComponents();

            var needsInitialization = container.Resolve<IEnumerable<INeedInitialization>>();
            await Task.WhenAll(needsInitialization.Select(x => x.Initialize()));

            var delegator = container.Resolve<RequestDelegator>();
            await delegator.PopulateRequestsAsync(from, to);

            Console.WriteLine("[App]: Press any key to start simulation...");
            Console.ReadKey();
            Console.WriteLine(" ");

            var simulator = container.Resolve<Simulation>();
            simulator.Subscribe(delegator);
            simulator.SetLoadStrategyEffectRate(1.0);
            simulator.RunSimulation(from, to);

            Console.WriteLine("[App]: Waiting 10.000 ms for any request to finish");
            await Task.Delay(10000);

            var storages = container.Resolve<IEnumerable<ISimulationResultStorage>>();
            foreach (var storage in storages)
            {
                try
                {
                    await storage.Save(SimulationTelemetry.Instance.GetRequestData().ToList());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[App]: Something went bad when trying to store result. Exception was: {ex.ToString()}");
                }
            }

            
            
        }

        private IContainer InitializeComponents()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => BuildConfiguration()).As<IConfiguration>();

            //builder.RegisterType<ChangeHostInterceptor>().As<IHttpRequestMessageInterceptor>();
            builder.RegisterType<AuthorizationInterceptor>().As<IHttpRequestMessageInterceptor>();

            builder.RegisterType<TokenStore>().As<INeedInitialization>().SingleInstance();
            builder.RegisterType<TokenStore>().As<ITokenStore>().SingleInstance();

            builder.RegisterType<RequestExecutor>().As<IRequestExecutor>();
            builder.RegisterType<ApplicationInsightsConfiguration>().SingleInstance();
            builder.RegisterType<Simulation>().SingleInstance();
            builder.RegisterType<RequestDelegator>().SingleInstance();
            builder.RegisterType<SqlSimulationResultStorage>().As<ISimulationResultStorage>();
            builder.RegisterType<ApplicationInsightsResultStorage>().As<ISimulationResultStorage>().PreserveExistingDefaults();

            builder.Register(x => new ConstantLoadStrategy(50)).As<ILoadStrategy>();
            //builder.RegisterType<LinearLoadStrategy>().As<ILoadStrategy>().WithParameter("slope", 1.5);

            builder.RegisterType<FileContentClient>().As<IContentClient>();
            //builder.RegisterType<ApplicationInsightsRequestDataSource>().As<IRequestDataSource>();
            builder.RegisterType<ApplicationInsightsDependencyDataSource>().As<IRequestDataSource>();
            builder.RegisterType<ApplicationInsightsIngestion>().As<IApplicationInsightsIngestion>();

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