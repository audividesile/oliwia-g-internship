using System;
using VigilantMeerkat.Micro.Base.Config;
using StackExchange.Redis;
using VigilantMeerkat.Micro.Base.Vault;

namespace VigilantMeerkat.Micro.Base.Cache
{
    public class RedisConnector
    {
        private readonly RedisConfig _configDefinition;
        private readonly Lazy<ConnectionMultiplexer> _connection;

        public RedisConnector(RedisConfig configDefinition)
        {
            _configDefinition = configDefinition;
            _connection = new Lazy<ConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(_configDefinition.ConnectionString)
            );
        }

        public ConnectionMultiplexer Connect()
        {
            return _connection.Value;
        }
    }
}