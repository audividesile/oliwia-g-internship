using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Base.Test.Micro
{
    public class ProtectedMicroservice : MicroserviceBase
    {
        [MicroRoute("get")]
        [Authorize]
        public CommonValue A(byte[] request, MessageContext context)
        {
            return new CommonValue
            {
                Value = 120.ToString()
            };
        }
    }
}