using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Base.Test.Micro
{
    public class StringMicroservice : MicroserviceBase
    {
        [MicroRoute("get")]
        public CommonValue Get(byte[] request, MessageContext context)
        {
            return new CommonValue
            {
                Value = "Test"
            };
        }
    }
}