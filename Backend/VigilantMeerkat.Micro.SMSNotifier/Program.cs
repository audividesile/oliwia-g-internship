using System;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.SMSNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<SMSNotifierMicroservice>.Host(args, opts =>
                opts.UseDbContext<AppDbContext>("app"));
        }
    }
}
