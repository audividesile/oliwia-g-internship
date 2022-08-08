using System;
using System.Collections.Generic;
using System.Text;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Authorization.Test.Micro
{
    public class ChainedMicroservice : MicroserviceBase
    {
        [MicroRoute("chain")]
        [Authorize]
        public CommonValue Chain(byte[] request, MessageContext context)
        {
            var res = Amqp.Call("simple", "get", context);

            return CommonValue.Parser.ParseFrom(res.Body);
        }
    }
}
