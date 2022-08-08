using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VigilantMeerkat.Db.Model
{
    [Table("adminconfig")]
    public class AdminConfig
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("phonenumber")]
        public string PhoneNumber { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("defaultnotificationtype")]
        public string DefaultNotificationType { get; set; }
    }
}
