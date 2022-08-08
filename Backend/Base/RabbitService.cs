using System.Reflection.Metadata;
using VigilantMeerkat.Micro.Base.Bind;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Context;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VigilantMeerkat.Micro.Base
{
    public class RabbitService
    {
        protected IRabbitConnection rabbitConnection;
        
        protected RabbitService(IRabbitConnection rabbitConnection)
        {
            this.rabbitConnection = rabbitConnection;
        }

        protected IModel CreateChannel(IConnection connection)
        {
            return connection.CreateModel();
        }

        protected QueueDeclareOk DeclareQueue(IModel channel, string name = "")
        {
            return channel.QueueDeclare(name);
        }

        protected IBasicProperties CreateProperties(IModel channel, string replyTo)
        {
            var props = channel.CreateBasicProperties();

            props.ReplyTo = replyTo;
            
            return props;
        }

        protected void Publish(IModel channel, string replyQueue, string microservice, string route, byte[] data)
        {
            channel.BasicPublish("", 
                GetRoutingKey(microservice, route), 
                false, 
                CreateProperties(channel, replyQueue), 
                data);
        }

        protected string GetRoutingKey(string microservice, string route) => $"{microservice}.{route}";
    }
}