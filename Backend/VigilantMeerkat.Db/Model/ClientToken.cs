using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VigilantMeerkat.Db.Model
{
    [Table("token")]
    public class ClientToken
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("userid")]
        public Guid UserId { get; set; }

        [Column("token")]
        public Guid Token { get; set; }

        [Column("adminid")]
        public Guid AdminId { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
