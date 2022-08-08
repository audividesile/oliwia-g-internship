using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Attribute;

namespace VigilantMeerkat.Micro.Presenter
{
    public class PresenterMicroservice : MicroserviceBase
    {
        [MicroRoute("logs", typeof(CommonValue))]
        public async Task<LogInfoList> GetLogsForUser(CommonValue request, MessageContext context)
        {
            var response = new LogInfoList();

            var tokens = await DbStore.Get<AppDbContext>().Tokens.Where(x => x.UserId == Guid.Parse(request.Value))
                .ToListAsync();

            Console.WriteLine("test1");

            foreach (var token in tokens)
            {
                var logs = await DbStore.Get<AppDbContext>().Logs.Where(x => x.TokenId == token.Token).ToListAsync();

                foreach (var log in logs)
                {
                    response.List.Add(new LogInfo
                    {
                        ClientId = log.TokenId.ToString(),
                        ClientName = token.Name,
                        CpuUsage = log.CpuUsage,
                        RamUsage = log.RamUsage,
                        Id = log.Id.ToString(),
                        UserId = request.Value,
                        Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(log.Timestamp, DateTimeKind.Utc))
                    });
                }
            }

            return response;
        }

        [MicroRoute("logsformeerkat", typeof(CommonValue))]
        public async Task<MeerkatLogList> GetLogsForMeerkat(CommonValue request, MessageContext context)
        {
            var response = new MeerkatLogList();

            var meerkat = await DbStore.Get<AppDbContext>().Tokens.FirstOrDefaultAsync(x => x.Token == Guid.Parse(request.Value));

            var tokens = await DbStore.Get<AppDbContext>().Logs.Where(x => x.TokenId == Guid.Parse(request.Value)).ToListAsync();

            foreach (var token in tokens)
            {
                response.List.Add(new MeerkatLog
                {
                    Id = token.Id.ToString(),
                    CpuUsage = token.CpuUsage,
                    RamUsage = token.RamUsage,
                    Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(token.Timestamp, DateTimeKind.Utc)),
                    Type = token.Type,
                    Name = meerkat.Name
                });
            }

            return response;
        }

        [MicroRoute("clients", typeof(CommonValue))]
        public async Task<AdminClientList> GetAdminClients(CommonValue request, MessageContext context)
        {
            var response = new AdminClientList();

            var clients = DbStore.Get<AppDbContext>().Tokens.Where(x => x.AdminId == Guid.Parse(request.Value)).ToList();

            foreach (var client in clients)
            {
                response.List.Add(new AdminClient
                {
                    UserId = client.UserId.ToString(),
                    MeerkatId = client.Token.ToString(),
                    Name = client.Name
                });
            }

            return response;
        }

        [MicroRoute("meerkatsbyuser", typeof(CommonValue))]
        public async Task<MeerkatInfoList> MeerkatsByUser(CommonValue request, MessageContext context)
        {
            var response = new MeerkatInfoList();

            var id = Guid.Parse(request.Value);

            Console.WriteLine("test1");

            var meerkats = await DbStore.Get<AppDbContext>().Tokens.Where(x => x.UserId == id).ToListAsync();

            Console.WriteLine("test2");

            foreach(var meerkat in meerkats)
            {
                Console.WriteLine("test3");
                var admin = await DbStore.Get<AppDbContext>().AdminConfigs.FirstOrDefaultAsync(x => x.Id == meerkat.AdminId);
                var cfg = await DbStore.Get<AppDbContext>().ClientConfigs.FirstOrDefaultAsync(x => x.ClientId == meerkat.Id);

                response.List.Add(new MeerkatInfo
                {
                    Id = meerkat.Id.ToString(),
                    Name = meerkat.Name,
                    Token = meerkat.Token.ToString(),
                    AdminInfo = new AdminInfo
                    {
                        Id = admin.Id.ToString(),
                        Email = admin.Email,
                        Phone = admin.PhoneNumber,
                        TriggerLevel = cfg.TriggerLevel
                    }
                });
            }

            Console.WriteLine("test4");

            return response;
        }
    }
}
