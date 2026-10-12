using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StatsEngineAPI.Domain.Interfaces.Services.Cripto;

namespace StatsEngineAPI.Worker
{
    public class DailyMarketDataWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailyMarketDataWorker> _logger;

        public DailyMarketDataWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<DailyMarketDataWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                    "Iniciando coleta diária de dados em {Date}",
                    DateTime.Now);

            using var timer = new PeriodicTimer(
                TimeSpan.FromMinutes(1));

            await ExecuteDailyJobAsync(stoppingToken);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ExecuteDailyJobAsync(stoppingToken);
            }
        }

        private async Task ExecuteDailyJobAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando coleta de dados em {Date}",
                    DateTime.Now);

                using var scope = _scopeFactory.CreateScope();

                var service = scope.ServiceProvider
                    .GetRequiredService<ICollectionDailyMarketDataService>();

                await service.CollectDailyDataAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Coleta diária finalizada em {Date}",
                    DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro durante a coleta diária.");
            }
        }
    }
}
