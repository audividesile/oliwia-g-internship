using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Micro.Meerkat
{
    public class MeerkatService : BackgroundService, IHostedService, IDisposable
    {
        private readonly AmqpService _amqp;
        private readonly TokenConfig _tokenConfig;

        public MeerkatService(AmqpService amqp, IOptions<TokenConfig> tokenConfig)
        {
            _amqp = amqp;
            _tokenConfig = tokenConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var rand = new Random();
                var data = ProvideData(rand.Next(100), rand.Next(100));
                _amqp.Call("central", "put", new MessageContext
                {
                    Token = _tokenConfig.Value
                }, data.ToByteArray());
                await Task.Delay(5000);
            }
        }

        private MeerkatData ProvideData(int cpuCounter, int ramCounter)
        {
            var type = "LOG";

            if(cpuCounter > 50)
            {
                type = "WARN";
            }
            else if (cpuCounter > 75)
            {
                type = "ERROR";
            }

            return new MeerkatData
            {
                Cpu = cpuCounter.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Ram = ramCounter.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
                Type = type
            };
        }
    }
}
