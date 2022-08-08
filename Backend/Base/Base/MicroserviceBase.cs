

using VigilantMeerkat.Micro.Base.Cache;
using VigilantMeerkat.Micro.Base.Ref;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VigilantMeerkat.Micro.Base.Db;
using Microsoft.Extensions.Configuration;
using VigilantMeerkat.Micro.Base.Logger;

namespace VigilantMeerkat.Micro.Base
{
    public abstract class MicroserviceBase 
    {
        public AmqpService Amqp { get; set; }

        public EFContextStore DbStore { get; set; }
        
        public AuthorizationRef AuthorizationRef { get; set; }
        
        public RedisConnector Redis { get; set; }

        public IConfiguration Configuration { get; set; }
        
        public virtual void Init()
        {
            
        }
    }
}