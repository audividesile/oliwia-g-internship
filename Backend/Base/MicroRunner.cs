

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base.Attribute;
using VigilantMeerkat.Micro.Base.Bind;
using VigilantMeerkat.Micro.Base.Cache;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Context;
using VigilantMeerkat.Micro.Base.Logger;
using VigilantMeerkat.Micro.Base.Ref;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp;
using VigilantMeerkat.Micro.Base.Vault;

namespace VigilantMeerkat.Micro.Base
{
    public class MicroRunner<T> : IHostedService, IDisposable where T : MicroserviceBase, new()
    {
        private readonly Dictionary<string, MethodBinding> _routes;
        private readonly MicroserviceContext _microserviceContext;
        private readonly T _instance;
        private readonly RabbitInternalService _rabbitInternalService;
        private readonly IRabbitConnection _rabbitConnection;

        public MicroRunner(AuthorizationRef authorizationRef, MicroserviceContext context, 
            RabbitInternalService rabbitInternalService, IRabbitConnection rabbitConnection, AmqpService amqp, MicroOptions options, IConfiguration configuration)
        {
            _routes = new Dictionary<string, MethodBinding>();

            _instance = new T
            {
                Redis = options.GetRedis(),
                DbStore = options.GetEFStore(),
                Amqp = amqp,
                AuthorizationRef = authorizationRef,
                Configuration = configuration
            };

            _rabbitConnection = rabbitConnection;
            _microserviceContext = context;
            _rabbitInternalService = rabbitInternalService;
        }

        public void Run()
        {
            DefineRoutes();
            CreateQueues();
            InitProperties();
        }

        public static async Task HostAsync(string[] args, Action<MicroOptions> options = null)
        {
            await BuildHost(args, options).RunAsync();
        }

        public static IHost BuildHost(string[] args, Action<MicroOptions> options = null)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());

                    configBuilder.AddEnvironmentVariables();

                    if(Environment.GetEnvironmentVariable("MICRO_RUN_TYPE") == "DEBUG")
                    {
                        configBuilder.AddJsonFile("config.dev.json");
                    }
                    else
                    {
                        configBuilder.AddJsonFile("config.prod.json");
                    }
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {

                    ConfigHelper.Configuration = hostBuilderContext.Configuration;

                    services.AddTransient(serviceProvider =>
                    {
                        return new MicroserviceContext
                        {
                            Name = typeof(T).Name.Replace("Microservice", "").ToLower()
                        };
                    });

                    services.Configure<ConfigDefinition>(hostBuilderContext.Configuration);
                    services.Configure<RabbitConfig>(hostBuilderContext.Configuration.GetSection("Rabbit"));
                    services.Configure<MongoConfig>(hostBuilderContext.Configuration.GetSection("Mongo"));
                    services.Configure<VaultConfig>(hostBuilderContext.Configuration.GetSection("Vault"));
                    services.Configure<RedisConfig>(hostBuilderContext.Configuration.GetSection("Redis"));
                    services.Configure<LogstashConfig>(hostBuilderContext.Configuration.GetSection("Logstash"));

                    services.AddTransient<ConfigLoader>();

                    services.AddTransient<AmqpService>();
                    services.AddSingleton<MongoDBLogger>();
                    services.AddTransient<AuthorizationRef>();
                    services.AddSingleton<IRabbitConnection, RabbitConnection>();
                    services.AddTransient<RabbitInternalService>();

                    services.AddSingleton<IAuthMethodInfo>((serviceProvider) =>
                    {
                        var cfg = serviceProvider.GetRequiredService<IOptions<VaultConfig>>();

                        return new TokenAuthMethodInfo(cfg.Value.Token);
                    });

                    services.AddSingleton((serviceProvider) =>
                    {
                        var cfg = serviceProvider.GetService<IOptions<VaultConfig>>();
                        var auth = serviceProvider.GetService<IAuthMethodInfo>();

                        return new VaultClientSettings(cfg.Value.Host, auth);
                    });

                    services.AddTransient<IVaultClient, VaultClient>();
                    services.AddTransient<VaultService>();

                    if (options != null)
                    {
                        var sp = services.BuildServiceProvider();

                        var opts = new MicroOptions(sp.GetService<IOptions<ConfigDefinition>>(), sp.GetService<MicroserviceContext>(), hostBuilderContext.Configuration);

                        options(opts);

                        services.AddTransient(x => opts);
                    }
                    else
                    {
                        services.AddTransient<MicroOptions>();
                    }

                    services.AddHostedService<MicroRunner<T>>();
                })
                .Build();
        }

        public static void Host(string[] args, Action<MicroOptions> options = null)
        {
            BuildHost(args, options).Run();
        }

        private void InitProperties()
        {
            _instance.Init();
        }

        private void CreateQueues()
        {
            foreach (var route in _routes)
            {
                Task.Run(async () =>
                {
                    await _rabbitInternalService.CreateQueue($"{_microserviceContext.Name}.{route.Key}", new QueueContext
                    {
                        Binding = route.Value,
                        Instance = _instance
                    });
                });
            }
        }

        private void DefineRoutes()
        {
            var routeMethods = GetRouteMethods(typeof(T));

            foreach (var route in routeMethods)
            {
                var attr = GetRouteAttribute(route);

                _routes.Add(attr.Route, new MethodBinding()
                {
                    Method = route,
                    ProtoType = attr.ProtoType,
                    IsAuth = IsMethodAuth(route)
                });
            }
        }

        private bool IsMethodAuth(MethodInfo method) => method.GetCustomAttributes(typeof(AuthorizeAttribute)).Any();

        private MethodInfo[] GetRouteMethods(Type type) => type
            .GetMethods()
            .Where(x => x.GetCustomAttributes(typeof(MicroRouteAttribute), false).Length > 0)
            .ToArray();

        private MicroRouteAttribute GetRouteAttribute(MethodInfo method) => method
            .GetCustomAttributes(typeof(MicroRouteAttribute), false)
            .FirstOrDefault() as MicroRouteAttribute;

        public void Dispose()
        {
            _rabbitConnection.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}