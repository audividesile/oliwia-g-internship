using System;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.EmailNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<EmailNotifierMicroservice>.Host(args, opts =>
                opts.UseDbContext<AppDbContext>(
                    (def, conf) => conf["AppConnectionString"]));
        }
    }
}
