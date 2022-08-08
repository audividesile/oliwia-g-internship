using System;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.Notifier
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<NotifierMicroservice>.Host(args, opts =>
                opts.UseDbContext<AppDbContext>(
                    (def, conf) => conf["AppConnectionString"]));
        }
    }
}
