using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Application.Services;
using StatsEngineAPI.Application.Services.AssetAnalysis;
using StatsEngineAPI.Application.Services.Dividends;
using StatsEngineAPI.Application.Services.MarketNews;
using StatsEngineAPI.Application.Services.Price;
using StatsEngineAPI.Domain.Interfaces.Services.Dividends;
using StatsEngineAPI.Domain.Models.MarketNews;
using StatsEngineAPI.Infrastructure.DependencyInjection;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;
using System.Runtime.CompilerServices;
using StatsEngineAPI.Infrastructure.MarketDataCentralizer;
using OutroProjeto.Infrastructure.DependencyInjection;
using StatsEngineAPI.Application.Services.Dividends.MarketDataCentralizer;
using StatsEngineAPI.Application.Services.Cripto;
using StatsEngineAPI.Worker;

namespace StatsEngineAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IDividendStatsticService, DividendStatisticsService>();       

            // Helpers
            builder.Services.AddScoped<ParseDecimalHelper>();
            builder.Services.AddScoped<ParseDataHelper>();

            // Calc Services
            builder.Services.AddScoped<CalcYieldService>();
            builder.Services.AddScoped<CalcGrowthService>();
            builder.Services.AddScoped<CalcConsistencyService>();

            // Statistics
            builder.Services.AddScoped<DividendStatisticsService>();
            builder.Services.AddScoped<GenerateStatsService>();

            //MarketNews
            builder.Services.AddSingleton<MarketNewsService>();
            builder.Services.AddSingleton<InfoLevelMessages>();

            builder.Services.AddScoped<NewsService>();

            //MarketInteligence
            builder.Services.AddScoped<AssetAnalysisService>();
            builder.Services.AddScoped<AssetScoringService>();
            builder.Services.AddScoped<PriceService>();

            //MarketDataCentralizer
            builder.Services.AddScoped<DividendsService>();
            builder.Services.AddScoped<IbovespaService>();


            builder.Services.AddMarketDataCentralizerInfrastructure(builder.Configuration);


            builder.Services.AddInfrastructure(builder.Configuration);

            //BackgoundServices / Workers
            builder.Services.AddScoped<CollectionDailyMarketDataService>();
            builder.Services.AddHostedService<DailyMarketDataWorker>();




            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddSwaggerGen(options =>
            {
                var provider = builder.Services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = $"StatsEngineAPI {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    });
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpper()
                        );
                    }
                });
            }

            app.UseHttpsRedirection();

            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
