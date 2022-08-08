using System;
using System.Collections.Generic;
using System.Text;
using VigilantMeerkat.Micro.Base.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace VigilantMeerkat.Micro.Base
{
    public class RabbitConnection : IDisposable, IRabbitConnection
    {
        private IConnection connection;

        public RabbitConnection(IOptions<RabbitConfig> configDefinition)
        {
            var conn = new ConnectionFactory
            {
                HostName = configDefinition.Value.HostName,
                Password = configDefinition.Value.Password
            };

            connection = conn.CreateConnection();
        }

        public IConnection GetConnection() => connection;

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}
