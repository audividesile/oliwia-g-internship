using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VigilantMeerkat.Gateway.Auth
{
    public class JwtToken
    {
        public string Token { get; set; }
        public long ExpiresAt { get; set; }

        public JwtToken(string token, long expiresAt)
        {
            Token = token;
            expiresAt = ExpiresAt;
        }
    }
}
