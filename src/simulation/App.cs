﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using RequestSimulation.Configuration;
using RequestSimulation.Datasources;
using RequestSimulation.Executing;
using RequestSimulation.Executing.Interceptors;
using RequestSimulation.Loadstrategies;

namespace RequestSimulation
{
    public class App
    {
        public async Task Run()
        {
            var from = new DateTime(2017, 09, 20, 13, 00, 00);
            var to = from.AddHours(1);

            Console.WriteLine(" ");
            Console.WriteLine($"Start:\t {from.ToString()} (UTC)");
            Console.WriteLine($"To:\t {to.ToString()} (UTC)");
            Console.WriteLine(" ");

            var container = InitializeComponents();

            var delegator = container.Resolve<RequestDelegator>();
            await delegator.PopulateRequestsAsync(from, to);

            var simulator = container.Resolve<Simulation>();
            simulator.Subscribe(delegator);
            simulator.SetLoadStrategyEffectRate(1.0);
            simulator.RunSimulation(from, to);
        }

        private IContainer InitializeComponents()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => BuildConfiguration()).As<IConfiguration>();
            builder.RegisterType<AuthorizationInterceptor>().As<IHttpRequestMessageInterceptor>();
            builder.RegisterType<ChangeHostInterceptor>().As<IHttpRequestMessageInterceptor>();
            builder.RegisterType<ApplicationInsightsDependencyDataSource>().As<IRequestDataSource>();
            builder.RegisterType<RequestExecutor>().As<IRequestExecutor>();
            builder.RegisterType<LinearLoadStrategy>().As<ILoadStrategy>().WithParameter("slope", 1.5);
            builder.RegisterType<TokenStore>().As<ITokenStore>();
            builder.RegisterType<FileContentClient>().As<IContentClient>();
            builder.RegisterType<ApplicationInsightsConfiguration>();
            builder.RegisterType<Simulation>();
            builder.RegisterType<RequestDelegator>();

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