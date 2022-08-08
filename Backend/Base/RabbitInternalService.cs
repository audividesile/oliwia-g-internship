using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Context;
using VigilantMeerkat.Micro.Base.Logger;
using VigilantMeerkat.Micro.Base.Ref;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VigilantMeerkat.Micro.Base.Vault;

namespace VigilantMeerkat.Micro.Base
{
    public class RabbitInternalService : RabbitService
    {
        private readonly AuthorizationRef _authorizationRef;

        public RabbitInternalService(VaultService vaultService,
            AuthorizationRef authorizationRef,
            IRabbitConnection rabbitConnection) : base(rabbitConnection)
        {
            _authorizationRef = authorizationRef;
        }

        public async Task CreateQueue(string name, QueueContext context)
        {
            using var channel = CreateChannel(rabbitConnection.GetConnection());

            DeclareQueue(channel, name);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, args) => { HandleRequest(model, args, context); };

            channel.BasicConsume(consumer, name);

            await Task.Delay(Timeout.Infinite);
        }

        private void HandleRequest(object model, BasicDeliverEventArgs args, QueueContext context)
        {
            var sp = args.BasicProperties.ReplyTo != null ? args.BasicProperties.ReplyTo.Split(".") : null;
            var reqMsg = GetRequestMessage(args.Body.ToArray());

            if (context.Binding.IsAuth)
                if (!_authorizationRef.Verify(reqMsg.Token))
                {
                    new AmqpService(rabbitConnection).Call(sp[0], sp[1], null, new AmqpMessage
                    {
                        Result = AmqpResult.Failed,
                        Body = new Error
                        {
                            Type = "Auth",
                            Message = "Unauthorized"
                        }.ToByteString()
                    }.ToByteArray(), false, false);
                    return;
                }

            object result = null;
            Exception exc = null;
            var isFailed = false;
            AmqpMessage msg = null;

            try
            {
                if (context.Binding.Method.ReturnType.Name == "Task`1")
                {
                    var task = (Task) context.Binding.Method.Invoke(context.Instance,
                        GetParametersForHandle(args, context, reqMsg.Token));
                    task.Wait();
                    result = task.GetType().GetProperty("Result").GetValue(task);
                }
                else
                {
                    result = context.Binding.Method.Invoke(context.Instance, GetParametersForHandle(args, context, reqMsg.Token));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                isFailed = true;
                exc = ex;
                result = new Error
                {
                    Message = ex.Message,
                    Stacktrace = ex.StackTrace,
                    Type = ex.GetType().Name
                };
            }

            if (args.BasicProperties.ReplyTo == null) return;

            if (isFailed)
                msg = new AmqpMessage
                {
                    Result = AmqpResult.Failed,
                    Body = ParseResponse(result)
                };
            else
                msg = new AmqpMessage
                {
                    Result = AmqpResult.Success,
                    Body = ParseResponse(result)
                };

            if (sp == null)
                return;

            new AmqpService(rabbitConnection).Call(sp[0], sp[1], null, msg.ToByteArray(), false, false);
        }

        private ByteString ParseResponse(object response)
        {

            if (response == null)
                return ByteString.Empty;

            var returnType = typeof(MessageExtensions);

            var toByteString = returnType.GetMethod("ToByteString");

            return toByteString.Invoke(response, new[] {response}) as ByteString;
        }

        private object[] GetParametersForHandle(BasicDeliverEventArgs args, QueueContext context, string token)
        {
            var res = new List<object>();

            if (context.Binding.ProtoType == null)
            {
                res.Add(args.Body.ToArray());
            }
            else
            {
                var parserProp = context.Binding.ProtoType.GetProperty("Parser").GetValue(null);
                var parserFromParams = new[] {typeof(byte[])};
                var parseFrom = parserProp.GetType().GetMethod("ParseFrom", 0, parserFromParams);

                res.Add(parseFrom.Invoke(parserProp, new object[] { GetBody(args.Body.ToArray()) }));
            }

            res.Add(GetMessageContext(context, args, token));

            return res.ToArray();
        }

        private byte[] GetBody(byte[] data) => GetRequestMessage(data).Body.ToByteArray();

        private AmqpMessage GetRequestMessage(byte[] data)
        {
            return AmqpMessage.Parser.ParseFrom(data);
        }

        private MessageContext GetMessageContext(QueueContext context, BasicDeliverEventArgs args, string token)
        {
            return new MessageContext
            {
                Route = args.BasicProperties.ReplyTo != null ? args.BasicProperties.ReplyTo : "",
                Token = token
            };
        }
    }
}