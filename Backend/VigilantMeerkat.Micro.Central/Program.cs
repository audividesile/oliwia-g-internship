using System;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.Central
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<CentralMicroservice>.Host(args, opts =>
                opts.UseDbContext<AppDbContext>(
                    (def, conf) => conf["AppConnectionString"]));
        }
    }
}
