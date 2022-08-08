using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Context;
using VigilantMeerkat.Micro.Base.Logger;
using VigilantMeerkat.Micro.Base.Ref;

namespace VigilantMeerkat.Micro.Meerkat
{
    class Program
    {
        static void Main(string[] args)
        {
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configBuilder.AddJsonFile("config.json");
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.Configure<RabbitConfig>(hostBuilderContext.Configuration.GetSection("Rabbit"));
                    services.Configure<TokenConfig>(hostBuilderContext.Configuration.GetSection("Token"));
                    services.AddTransient<AmqpService>();
                    services.AddSingleton<IRabbitConnection, RabbitConnection>();

                    services.AddHostedService<MeerkatService>();
                })
                .Build().Run();
        }
    }
}
