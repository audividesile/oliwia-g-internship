using System;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Base.Test.Micro
{
    public class ExceptionMicroservice : MicroserviceBase
    {
        [MicroRoute("get")]
        public CommonValue A(byte[] request, MessageContext context)
        {
            throw new NotImplementedException();
        }
    }
}