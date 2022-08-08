using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VigilantMeerkat.Micro.Test
{
    public class JwtProvider
    {

        public JwtProvider()
        {
        }

        public string GenerateToken()
        {

            var signingCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes("teaudjfgdsufdsaiuhaudfgndfg")),
                algorithm: SecurityAlgorithms.HmacSha256
            );

            DateTime jwtDate = DateTime.Now;

            var jwt = new JwtSecurityToken(
                audience: "https://localhost:4200",
                issuer: "https://localhost:11000",
                claims: GetUserClaims(),
                notBefore: jwtDate,
                expires: jwtDate.AddDays(10),
                signingCredentials: signingCredentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        private Claim[] GetUserClaims()
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.Name, "test"),
                new Claim("id", Guid.NewGuid().ToString()),
                new Claim("role", "ADMIN")
            };
        }
    }
}
