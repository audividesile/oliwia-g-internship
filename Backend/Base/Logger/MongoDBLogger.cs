using System;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VigilantMeerkat.Micro.Base.Vault;

namespace VigilantMeerkat.Micro.Base.Logger
{
    public class MongoDBLogger : ILogger
    {
        private readonly MongoConfig _config;
        private readonly MongoConnector _mongoConnector;
        private readonly MicroserviceContext _microserviceContext;
        
        public MongoDBLogger(IOptions<MongoConfig> config, MicroserviceContext microserviceContext)
        {
            _config = config.Value;
            _microserviceContext = microserviceContext;
            _mongoConnector = new MongoConnector(config.Value);
        }
        
        public void Log(LogLevel logLevel, LogEntry obj)
        {
            Log(logLevel, new EventId(), obj, null, null);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _mongoConnector.Insert(state, _microserviceContext.Name.ToLower());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _config.LogLevel == logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}