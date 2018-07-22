using Autofac;
using BlockchainExample.Shared;
using Consul;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BlockchainExample.Server
{
    class Program
    {
        private static Blockchain blockchain = new Blockchain();

        static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var container = BuildContainer(configuration);

            var app = container.Resolve<App>();
            app.Start();
        }

        private static IContainer BuildContainer(IConfigurationRoot configuration)
        {
            var nodeIpAddress = configuration["Node:IpAddress"];
            var nodePort = int.Parse(configuration["Node:Port"]);

            var builder = new ContainerBuilder();

            builder.Register(c => new TcpServer(nodeIpAddress, nodePort)).SingleInstance();
            builder.RegisterType<ConsulClient>().As<IConsulClient>().SingleInstance();
            builder.RegisterType<App>();

            return builder.Build();
        }
    }
}
