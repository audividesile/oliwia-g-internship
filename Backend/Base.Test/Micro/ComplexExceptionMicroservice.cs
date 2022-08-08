using System;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Base.Test.Micro
{
    public class ComplexExceptionMicroservice : MicroserviceBase
    {
        [MicroRoute("get")]
        public CommonValue A(byte[] request, MessageContext context)
        {
            var res = Amqp.Call("exception", "get");

            if (res.Result == AmqpResult.Failed)
            {
                throw new NotImplementedException();
            }

            return null;
        }
    }
}