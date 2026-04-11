
using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Application.Services;
using StatsEngineAPI.Application.Services.Dividends;
using StatsEngineAPI.Domain.Interfaces.Services.Dividends;
using StatsEngineAPI.Infrastructure.DepedencyInjection;
using StatsEngineAPI.Infrastructure.Repository.Nasdaq;

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

            builder.Services.AddInfrastructure();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
