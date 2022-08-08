using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Context;

namespace VigilantMeerkat.Micro.Test
{
    public static class TestMicroRunner
    {

        public static async Task Run<T>() where T : MicroserviceBase, new()
        {
            await Task.Factory.StartNew(async () =>
            {
                await MicroRunner<T>.HostAsync(new string[] { }, opts => opts.UseRedis().UseDbContext<AppDbContext>("app"));
                await Task.Delay(Timeout.Infinite);
            }, TaskCreationOptions.LongRunning);
        }
    }
}
