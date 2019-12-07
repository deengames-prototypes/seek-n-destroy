using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SeekAndDestroy.Core.Game;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SeekAndDestroy.Infrastructure.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var ticker = new Ticker();
            while (!stoppingToken.IsCancellationRequested)
            {
                ticker.DoTick();
                await Task.Delay(int.Parse(this.Config()["MinutesPerIncrement"]) * 1000 * 60, stoppingToken);
            }
        }
        public IConfigurationRoot Config() => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
}
