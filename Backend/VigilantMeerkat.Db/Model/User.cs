using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VigilantMeerkat.Db.Model
{
    [Table("user")]
    public class User
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }
    }
}
