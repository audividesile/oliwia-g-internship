using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace VigilantMeerkat.Micro.Base
{
    public interface IRabbitConnection : IDisposable
    {
        IConnection GetConnection();
    }
}
