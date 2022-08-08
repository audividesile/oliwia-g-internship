using System;
using System.Collections.Generic;
using System.Text;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Authorization.Test.Micro
{
    public class SimpleMicroservice : MicroserviceBase
    {
        [MicroRoute("get")]
        [Authorize]
        public CommonValue Get(byte[] request, MessageContext context)
        {
            return new CommonValue
            {
                Value = "Test"
            };
        }
    }
}
