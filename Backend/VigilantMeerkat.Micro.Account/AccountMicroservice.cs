using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Account
{
    public class AccountMicroservice : MicroserviceBase
    {
        [MicroRoute("changeadminconfig", typeof(AdminConfigChange))]
        public async Task<BoolValue> ChangeAdminConfig(AdminConfigChange adminConfig, MessageContext context)
        {
            var db = DbStore.Get<AppDbContext>();
            var cfg = db.AdminConfigs.FirstOrDefault(x => x.Id == Guid.Parse(adminConfig.Id));

            if (cfg == null)
                return new BoolValue
                {
                    Value = false
                };

            if (!string.IsNullOrEmpty(adminConfig.DefaultNotificationType))
            {
                cfg.DefaultNotificationType = adminConfig.DefaultNotificationType;
            }

            if (!string.IsNullOrEmpty(adminConfig.Email))
            {
                cfg.Email = adminConfig.Email;
            }

            if (!string.IsNullOrEmpty(adminConfig.PhoneNumber))
            {
                cfg.PhoneNumber = adminConfig.PhoneNumber;
            }

            db.AdminConfigs.Update(cfg);
            await db.SaveChangesAsync();

            return new BoolValue
            {
                Value = true
            };
        }
    }
}
