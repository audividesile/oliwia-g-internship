using System;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.ClientCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<ClientCreatorMicroservice>.Host(args, opts => 
                opts.UseDbContext<AppDbContext>(
                    (def, conf) => conf["AppConnectionString"]));
        }
    }
}
