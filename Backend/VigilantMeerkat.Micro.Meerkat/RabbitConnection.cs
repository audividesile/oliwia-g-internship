using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.Meerkat
{
    public class RabbitConnection : IDisposable, IRabbitConnection
    {
        private IConnection connection;

        public RabbitConnection(IOptions<RabbitConfig> options)
        {

            var conn = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                Password = options.Value.Password
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
