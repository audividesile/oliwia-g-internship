using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Npgsql;
using VigilantMeerkat.Db;
using VigilantMeerkat.Db.Model;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.ClientCreator
{
    public class ClientCreatorMicroservice : MicroserviceBase
    {
        [MicroRoute("create", typeof(CommonValue))]
        public async Task<CommonValue> CreateToken(CommonValue request, MessageContext context)
        {
            Console.WriteLine("create1");
            var tok = new CommonValue
            {
                Value = context.Token
            };

            var token = Amqp.Call("authorization", "create", context, request.ToByteArray());

            Console.WriteLine("create2");
            var handler = new JwtSecurityTokenHandler();
            var tokenUser = handler.ReadToken(context.Token) as JwtSecurityToken;

            var id = tokenUser.Claims.FirstOrDefault(x => x.Type == "id");
            var tokenId = Guid.NewGuid();

            var newToken = new ClientToken
            {
                Id = tokenId,
                Token = Guid.Parse(CommonValue.Parser.ParseFrom(token.Body).Value),
                UserId = Guid.Parse(id.Value),
                AdminId = GetRandomAdmin(),
                Name = request.Value
            };

            var clientConfig = new ClientConfig
            {
                ClientId = tokenId,
                Id = Guid.NewGuid(),
                TriggerLevel = "ERROR"
            };

            Console.WriteLine("create3");

            await DbStore.Get<AppDbContext>().ClientConfigs.AddAsync(clientConfig);
            await DbStore.Get<AppDbContext>().Tokens.AddAsync(newToken);
            await DbStore.Get<AppDbContext>().SaveChangesAsync();

            Console.WriteLine("create4");

            return new CommonValue
            {
                Value = newToken.Token.ToString()
            };
        }

        private Guid GetRandomAdmin()
        {
            var count = DbStore.Get<AppDbContext>().AdminConfigs.Count();

            var rnd = new Random();

            var index = rnd.Next(count);

#if DEBUG
            if(index <= 0)
            {
                return Guid.NewGuid();
            }
#endif

            return DbStore.Get<AppDbContext>().AdminConfigs.ToList()[index].Id;
        }
    }
}
