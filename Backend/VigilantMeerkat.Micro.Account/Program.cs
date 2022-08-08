using System;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.Account
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<AccountMicroservice>.Host(args, opts =>
                opts.UseDbContext<AppDbContext>("app"));
        }
    }
}
