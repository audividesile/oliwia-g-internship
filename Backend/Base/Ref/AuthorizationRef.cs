using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace VigilantMeerkat.Micro.Base.Ref
{
    public class AuthorizationRef
    {
        private readonly AmqpService _amqp;

        public AuthorizationRef(AmqpService amqpService)
        {
            _amqp = amqpService;
        }

        public bool Verify(string token)
        {
            var res = _amqp.Call("authorization", "verify", new MessageContext(), new CommonValue
            {
                Value = token
            }.ToByteArray());

            var resp = BoolValue.Parser.ParseFrom(res.Body);

            if (resp.Value)
            {
                return true;
            }

            return false;
        }
    }
}