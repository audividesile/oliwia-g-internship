using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Base.Test.Micro
{
    public class ComplexMicroservice : MicroserviceBase
    {
        [MicroRoute("get", typeof(CommonValue))]
        public async Task<CommonValue> Get(CommonValue request, MessageContext context)
        {
            var number = Amqp.Call("concate", "get");

            var num = CommonValue.Parser.ParseFrom(number.Body);
            
            return new CommonValue
            {
                Value = num.Value + request.Value
            };
        }
    }
}