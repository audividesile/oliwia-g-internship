using System;
using System.Threading;
using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.Authorization
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroRunner<AuthorizationMicroservice>.Host(args, opts => opts.UseRedis());
        }
    }
}