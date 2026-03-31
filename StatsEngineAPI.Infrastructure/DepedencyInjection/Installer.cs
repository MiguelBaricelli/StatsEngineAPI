using Microsoft.Extensions.DependencyInjection;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;

namespace StatsEngineAPI.Infrastructure.DepedencyInjection
{
    public static class Installer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // 🔥 HttpClient (recomendado)
            services.AddScoped<AlphaDividendsConsumer>();

            // Se tiver outros consumers/repos, registre aqui
            // services.AddScoped<OutroRepository>();

            return services;
        }
    }
}
