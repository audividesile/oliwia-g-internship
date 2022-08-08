using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Google.Protobuf.WellKnownTypes;

namespace VigilantMeerkat.Micro.Authorization
{
    public class AuthorizationMicroservice : MicroserviceBase
    {
        [MicroRoute("verify", typeof(CommonValue))]
        public async Task<BoolValue> VerifyToken(CommonValue request, MessageContext context)
        {
            var red = Redis.Connect();

            var res = red.GetDatabase(0).KeyExists(new RedisKey(request.Value));

            if (res)
            {

                return new BoolValue
                {
                    Value = true
                };
            }
            else
            {

                return new BoolValue
                {
                    Value = false
                };
            }
        }

        [MicroRoute("create", typeof(CommonValue))]
        public async Task<CommonValue> CreateToken(CommonValue request, MessageContext context)
        {
            Console.WriteLine("create1");
            var red = Redis.Connect();
            Console.WriteLine("create2");
            var token = Guid.NewGuid().ToString();

            var res = red.GetDatabase(0).StringSet(new RedisKey(token), new RedisValue(CreateToken(request)));

            Console.WriteLine("create3");

            if (res)
            {


                return new CommonValue
                {
                    Value = token
                };
            }
            else
            {

                return new CommonValue
                {
                    Value = Guid.Empty.ToString()
                };
            }
        }

        private string CreateToken(CommonValue request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = new JwtSecurityToken
            (
                "jwt",
                "jwt",
                claims: new List<Claim>() { new Claim(ClaimTypes.Name, request.Value) },
                DateTime.Now.AddMilliseconds(-30),
                DateTime.Now.AddDays(60),
                new SigningCredentials(new SymmetricSecurityKey(Encoding.Default.GetBytes("testtesttesttesttesttesttesttesttesttest")), SecurityAlgorithms.HmacSha256)
            );

            return tokenHandler.WriteToken(token);
        }
    }
}