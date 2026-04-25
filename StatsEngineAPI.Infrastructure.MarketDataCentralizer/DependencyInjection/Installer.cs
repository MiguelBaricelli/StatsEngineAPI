using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StatsEngineAPI.Infrastructure.MarketDataCentralizer;
using StatsEngineAPI.Infrastructure.MarketDataCentralizer.Config;
using StatsEngineAPI.Infrastructure.Repository.MarketNews.Config;

namespace OutroProjeto.Infrastructure.DependencyInjection
{
    public static class Installer
    {
        public static IServiceCollection AddMarketDataCentralizerInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MarketDataCentralizerConfig>(configuration.GetSection("MarketDataCentralizerConfig"));

            var marketDataConfig = configuration.GetSection("MarketDataCentralizerConfig").Get<MarketNewsIntegrationConfig>()
                ?? throw new InvalidOperationException("Seção 'MarketDataCentralizerConfig' não encontrada no appsettings.json");

            if (string.IsNullOrWhiteSpace(marketDataConfig.BaseUrl))
                throw new InvalidOperationException("MarketDataCentralizerConfig:BaseUrl é obrigatório");

            if (marketDataConfig.Timeout <= 0)
                throw new InvalidOperationException("MarketDataCentralizerConfig:Timeout é obrigatório");

            services.AddHttpClient<MarketDataCentralizerIntegration>(client =>
            {
                client.BaseAddress = new Uri(marketDataConfig.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(marketDataConfig.Timeout > 0 ? marketDataConfig.Timeout : 30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddScoped<IMarketDataCentralizerIntegration, MarketDataCentralizerIntegration>();

            return services;
        }
    }
}
