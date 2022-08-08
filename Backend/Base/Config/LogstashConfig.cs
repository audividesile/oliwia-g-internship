using System;
using System.Collections.Generic;
using System.Text;

namespace VigilantMeerkat.Micro.Base.Config
{
    public class LogstashConfig
    {
        public LogstashConnectionConfig Exception { get; set; }
        public LogstashConnectionConfig Counter { get; set; }
    }
}
