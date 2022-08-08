using Microsoft.Extensions.Logging;

namespace VigilantMeerkat.Micro.Base.Config
{
    public class MongoConfig
    {
        public LogLevel LogLevel { get; set; }
        public string ConnectionString { get; set; }
    }
}