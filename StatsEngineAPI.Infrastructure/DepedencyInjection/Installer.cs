using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StatsEngineAPI.Infrastructure.Repository.MarketNews;
using StatsEngineAPI.Infrastructure.Repository.MarketNews.Config;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;
using Microsoft.Extensions.Http;

namespace StatsEngineAPI.Infrastructure.DependencyInjection
{
    public static class Installer
    {
        public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            var config = configuration
                .GetSection("MarketNews")
                .Get<MarketNewsIntegrationConfig>();

            services.AddSingleton(config);

            services.AddHttpClient<MarketNewsIntegration>((provider, client) =>
            {
                client.BaseAddress = new Uri(config.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(config.Timeout);
            });

            services.AddScoped<AlphaDividendsConsumer>();

            return services;
        }
    }
  }