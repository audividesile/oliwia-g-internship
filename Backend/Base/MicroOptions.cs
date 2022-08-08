using System;
using VigilantMeerkat.Micro.Base.Cache;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Context;
using VigilantMeerkat.Micro.Base.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VigilantMeerkat.Micro.Base.Db;
using VigilantMeerkat.Micro.Base.Vault;

namespace VigilantMeerkat.Micro.Base
{
    public class MicroOptions
    {
        private EFContextStore _efStore;
        private ILogger _logger;
        private RedisConnector _redisConnector;
        private readonly ConfigDefinition _config;
        
        private readonly MicroserviceContext _microserviceContext;
        private readonly IConfiguration _configuration;

        public MicroOptions(IOptions<ConfigDefinition> config, MicroserviceContext microserviceContext, IConfiguration configuration)
        {
            _microserviceContext = microserviceContext;
            _configuration = configuration;
            _config = config.Value;
            _efStore = new EFContextStore();
        }
        
        public MicroOptions UseDbContext<TDbContext>(Func<ConfigDefinition, IConfiguration, string> connectionString) where TDbContext : DbContext, new()
        {
            _efStore.Add(new TDbContext());

            ConfigHelper.ConnectionString.Add(typeof(TDbContext).Name, connectionString(_config, _configuration));

            return this;
        }

        public MicroOptions UseRedis()
        {
            _redisConnector = new RedisConnector(_config.Redis);

            return this;
        }

        public EFContextStore GetEFStore() => _efStore;

        public RedisConnector GetRedis() => _redisConnector;
    }
}