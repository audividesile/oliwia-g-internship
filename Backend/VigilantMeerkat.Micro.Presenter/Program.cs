using System;
using VigilantMeerkat.Db;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.Presenter
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<PresenterMicroservice>.Host(args, opts =>
                opts.UseDbContext<AppDbContext>(
                    (def, conf) => conf["AppConnectionString"]));
        }
    }
}
