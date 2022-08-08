using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base.Attribute;
using Microsoft.Extensions.Logging;

namespace VigilantMeerkat.Micro.Base.Test.Micro
{
    public class ConcateMicroservice : MicroserviceBase
    {
        [MicroRoute("get")]
        public async Task<CommonValue> Get(byte[] request, MessageContext context)
        {
            var number = Amqp.Call("number", "get");
            var str = Amqp.Call("string", "get");

            var num = CommonValue.Parser.ParseFrom(number.Body);
            var st = CommonValue.Parser.ParseFrom(str.Body);
            
            return new CommonValue
            {
                Value = num.Value + st.Value
            };
        }
    }
}