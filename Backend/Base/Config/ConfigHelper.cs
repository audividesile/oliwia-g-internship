using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace VigilantMeerkat.Micro.Base.Config
{
    public static class ConfigHelper
    {
        public static IConfiguration Configuration { get; set; }
        public static Dictionary<string, string> ConnectionString { get; private set; } = new Dictionary<string, string>();
    }
}
