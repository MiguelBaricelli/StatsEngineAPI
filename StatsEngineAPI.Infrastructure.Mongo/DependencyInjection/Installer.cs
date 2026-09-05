using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StatsEngineAPI.Infrastructure.Mongo.Configuration;

namespace StatsEngineAPI.Infrastructure.Mongo.DependencyInjection
{
    public static class Installer
    {
        public static IServiceCollection AddInfrastructureMongoDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration
                .GetSection("MongoDbConfig");

            var connectionString = section["ConnectionString"]
                ?? throw new InvalidOperationException(
                    "ConnectionString não encontrada.");

            var databaseName = section["DatabaseName"]
                ?? throw new InvalidOperationException(
                    "DatabaseName não encontrada.");

            services.AddSingleton<IMongoClient>(
                new MongoClient(connectionString));

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client =
                    sp.GetRequiredService<IMongoClient>();

                return client.GetDatabase(databaseName);
            });

            services.AddScoped<
                IMongoIntegration,
                MongoIntegration>();

            return services;
        }
    }
}
