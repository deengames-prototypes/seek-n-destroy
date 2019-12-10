using System.Threading;
using System.Threading.Tasks;
using SeekAndDestroy.Core.Game;
using SeekAndDestroy.Infrastructure.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SeekAndDestroy.Infrastructure.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfigurationRoot _configuration;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var userRepository = new UserRepository(_configuration["ConnectionString"]);
            var ticker = new Ticker(userRepository);
            while (!stoppingToken.IsCancellationRequested)
            {
                ticker.DoTick();
                await Task.Delay(int.Parse(_configuration["MinutesPerIncrement"]) * 1000 * 60, stoppingToken);
            }
        }
    }
}
