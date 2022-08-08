using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VigilantMeerkat.Gateway.Models;
using Microsoft.IdentityModel.Tokens;

namespace VigilantMeerkat.Gateway.Auth
{
    public class JwtProvider
    {
        private readonly IConfiguration _configuration;

        public JwtProvider(IConfiguration config)
        {
            _configuration = config;
        }

        public JwtToken GenerateToken(Account account)
        {

            var signingCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"])),
                algorithm: SecurityAlgorithms.HmacSha256
            );

            DateTime jwtDate = DateTime.Now;

            var jwt = new JwtSecurityToken(
                audience: "https://localhost:4200",
                issuer: "https://localhost:11000",
                claims: GetUserClaims(account),
                notBefore: jwtDate,
                expires: jwtDate.AddDays(10),
                signingCredentials: signingCredentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JwtToken(token, new DateTimeOffset(jwtDate).ToUnixTimeMilliseconds());
        }

        private Claim[] GetUserClaims(Account account)
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.Name, account.Email),
                new Claim("id", account.Id.ToString()), 
                new Claim("role", account.Role)
            };
        }
    }
}
