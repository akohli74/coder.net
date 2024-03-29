﻿using System;
using System.Threading.Tasks;
using coder.net.application;
using coder.net.configuration;
using coder.net.core;
using coder.net.transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace coder.net.console
{
    class Program
    {
        static void Main(string[] _)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var serviceProvider = ConfigureServices();

            _ = await Task.Run(serviceProvider.GetService<Bootstrapper>().Run).ConfigureAwait(false);

            serviceProvider.Dispose();
        }

        static IConfiguration BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(path: "./config.json", optional: false, reloadOnChange: true);
            return configBuilder.Build();
        }

        static ServiceProvider ConfigureServices()
        {
            var config = BuildConfiguration();
            var serverConfig = new ServerConfiguration();
            var clientConfig = new ClientConfiguration();

            config.Bind("ServerConfiguration", serverConfig);
            config.Bind("ClientConfiguration", clientConfig);

            IServiceCollection services = new ServiceCollection();
            services
                .AddTransient<ITcpServer, TcpServer>()
                .AddTransient<IClient, Client>()
                .AddTransient<IController, Controller>()
                .AddTransient<Bootstrapper>()
                .AddSingleton(serverConfig)
                .AddSingleton(clientConfig)
                .AddLogging(configure => configure.AddConsole());

            return services.BuildServiceProvider();
        }
    }
}
