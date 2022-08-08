using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VigilantMeerkat.Db.Model
{
    [Table("clientconfig")]
    public class ClientConfig
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("clientid")]
        public Guid ClientId { get; set; }

        [Column("triggerlevel")]
        public string TriggerLevel { get; set; }
    }
}
