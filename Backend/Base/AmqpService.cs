using System.Collections.Concurrent;
using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Transformation;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VigilantMeerkat.Micro.Base
{
    public class AmqpService : RabbitService
    {
        private readonly BlockingCollection<byte[]> response;

        public AmqpService(IRabbitConnection rabbitConnection) : base(rabbitConnection)
        {
            response = new BlockingCollection<byte[]>();
        }

        public AmqpMessage Call(string microservice, string route, MessageContext context = null, byte[] data = null, bool twoWay = true, bool pack = true)
        {
            using var channel = CreateChannel(rabbitConnection.GetConnection());

            if (twoWay)
            {
                var queue = DeclareQueue(channel);

                Publish(channel, queue.QueueName, microservice, route, pack ? PackRequest(data, context) : data);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += ResponseCallback;

                channel.BasicConsume(consumer, queue.QueueName);

                return ProtoTransformation.TransformMessage(response.Take());
            }

            Publish(channel, null, microservice, route, pack ? PackRequest(data, context) : data);
            return null;
        }

        private byte[] PackRequest(byte[] data, MessageContext context)
        {
            var token = context != null ? context.Token : "";
            var bytes = data != null ? ByteString.CopyFrom(data) : ByteString.CopyFrom();

            return new AmqpMessage
            {
                Body = bytes,
                Token = token,
                Result = AmqpResult.None
            }.ToByteArray();
        }

        private void ResponseCallback(object sender, BasicDeliverEventArgs args)
        {
            response.Add(args.Body.ToArray());
        }
    }
}