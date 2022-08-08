using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Base.Test.Micro
{
    public class NumberMicroservice : MicroserviceBase
    {
        [MicroRoute("get")]
        public CommonValue A(byte[] request, MessageContext context)
        {
            return new CommonValue
            {
                Value = 120.ToString()
            };
        }
    }
}