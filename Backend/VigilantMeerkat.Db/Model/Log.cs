using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VigilantMeerkat.Db.Model
{
    [Table("log")]
    public class Log
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tokenid")]
        public Guid TokenId { get; set; }

        [Column("cpuusage")]
        public double CpuUsage { get; set; }

        [Column("ramusage")]
        public double RamUsage { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
