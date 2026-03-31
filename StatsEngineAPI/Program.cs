
using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Application.Services.Dividends;
using StatsEngineAPI.Domain.Interfaces.Services.Dividends;
using StatsEngineAPI.Infrastructure.DepedencyInjection;

namespace StatsEngineAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IDividendStatsticService, DividendStatisticsService>();
            builder.Services.AddScoped<ParseDataHelper>();
            builder.Services.AddScoped<ParseDecimalHelper>();
            builder.Services.AddScoped<CalcGrowthService>();
            builder.Services.AddScoped<CalcYieldService>();
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
