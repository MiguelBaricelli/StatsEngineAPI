using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StatsEngineAPI.Domain.Interfaces.Infra.MarketNews;
using StatsEngineAPI.Infrastructure.Repository.B3;
using StatsEngineAPI.Infrastructure.Repository.MarketNews;
using StatsEngineAPI.Infrastructure.Repository.MarketNews.Config;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;

namespace StatsEngineAPI.Infrastructure.DependencyInjection
{
    public static class Installer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ══════════════════════════════════════════════
            // MarketNews (Marketaux)
            // ══════════════════════════════════════════════
            services.Configure<MarketNewsIntegrationConfig>(configuration.GetSection("MarketNews"));

            var marketNewsConfig = configuration.GetSection("MarketNews").Get<MarketNewsIntegrationConfig>()
                ?? throw new InvalidOperationException("Seção 'MarketNews' não encontrada no appsettings.json");

            if (string.IsNullOrWhiteSpace(marketNewsConfig.BaseUrl))
                throw new InvalidOperationException("MarketNews:BaseUrl é obrigatório");

            if (string.IsNullOrWhiteSpace(marketNewsConfig.ApiKey))
                throw new InvalidOperationException("MarketNews:ApiKey é obrigatório");

            services.AddHttpClient<MarketNewsIntegration>(client =>
            {
                client.BaseAddress = new Uri(marketNewsConfig.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(marketNewsConfig.Timeout > 0 ? marketNewsConfig.Timeout : 30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // ══════════════════════════════════════════════
            // AlphaVantage
            // ══════════════════════════════════════════════
            services.Configure<AlphaMarketNewsConfig>(configuration.GetSection("AlphaVantageConfig"));

            var alphaConfig = configuration.GetSection("AlphaVantageConfig").Get<AlphaMarketNewsConfig>()
                ?? throw new InvalidOperationException("Seção 'AlphaVantageConfig' não encontrada no appsettings.json");

            if (string.IsNullOrWhiteSpace(alphaConfig.BaseUrl))
                throw new InvalidOperationException("AlphaVantageConfig:BaseUrl é obrigatório");

            if (string.IsNullOrWhiteSpace(alphaConfig.ApiKey))
                throw new InvalidOperationException("AlphaVantageConfig:ApiKey é obrigatório");

            services.AddHttpClient<AlphaVantageMarketNewsIntegration>(client =>
            {
                client.BaseAddress = new Uri(alphaConfig.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(alphaConfig.Timeout > 0 ? alphaConfig.Timeout : 30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddScoped<IAlphaVantageMarketNewsIntegration, AlphaVantageMarketNewsIntegration>();

            // ══════════════════════════════════════════════
            // Outros serviços
            // ══════════════════════════════════════════════
            services.AddScoped<BrApiIntegration>();
            services.AddScoped<AlphaDividendsConsumer>();

            return services;
        }
    }
}