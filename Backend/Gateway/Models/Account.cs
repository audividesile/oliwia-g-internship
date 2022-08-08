using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VigilantMeerkat.Db.Model;

namespace VigilantMeerkat.Gateway.Models
{
    public class Account
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public string Role { get; set; }

        public Account(VigilantMeerkat.Db.Model.User user)
        {
            Email = user.Email;
            Id = user.Id;
            Role = user.Email.EndsWith("admin.pl") ? "ADMIN" : "USER";
        }
    }
}
