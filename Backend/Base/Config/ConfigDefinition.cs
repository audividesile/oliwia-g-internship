using Microsoft.Extensions.Logging;

namespace VigilantMeerkat.Micro.Base.Config
{
    public class ConfigDefinition
    {
        public DbConfig Db { get; set; }
        public RabbitConfig Rabbit { get; set; }
        public MongoConfig Mongo { get; set; }
        public RedisConfig Redis { get; set; }
    }
}