using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using VigilantMeerkat.Db;
using VigilantMeerkat.Db.Model;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Central
{
    public class CentralMicroservice : MicroserviceBase
    {
        [MicroRoute("put", typeof(MeerkatData))]
        [Authorize]
        public async Task<BoolValue> Put(MeerkatData request, MessageContext context)
        {
            await DbStore.Get<AppDbContext>().Logs.AddAsync(new Log
            {
                Id = Guid.NewGuid(),
                CpuUsage = Convert.ToDouble(request.Cpu, System.Globalization.CultureInfo.InvariantCulture),
                RamUsage = Convert.ToDouble(request.Ram, System.Globalization.CultureInfo.InvariantCulture),
                Type = request.Type,
                TokenId = Guid.Parse(context.Token),
                Timestamp = request.Timestamp.ToDateTime()
            });

            await DbStore.Get<AppDbContext>().SaveChangesAsync();

            var notification = new Notification
            {
                MeerkatId = context.Token,
                Message = $"[{request.Type}] CPU: {request.Cpu} RAM: {request.Ram}",
                Token = context.Token,
                Type = request.Type
            };

            Amqp.Call("notifier", "notify", context, notification.ToByteArray(), false);

            return new BoolValue
            {
                Value = true
            };
        }
    }
}
