using Microsoft.Extensions.Logging;
using StatsEngineAPI.Worker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Services.Cripto
{
    public class CollectionDailyMarketDataService
    {
        private readonly ILogger<CollectionDailyMarketDataService> _logger;
        public CollectionDailyMarketDataService(ILogger<CollectionDailyMarketDataService> logger)
        {
            _logger = logger;
        }

        public async Task CollectDailyDataAsync(CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();

            List<CriptoResponseModel> mockData = List<CriptoResponseModel>
            {
                new CriptoResponseModel
                {
                    Name = "Bitcoin",
                    DateInsert = DateTime.UtcNow,
                    Price = 50000.00,
                    MaxPrice = 51000.00,
                    MinPrice = 49000.00,
                };
                new CriptoResponseModel
                {
                    Name = "ETH",
                    DateInsert = DateTime.UtcNow,
                    Price = 100.00,
                    MaxPrice = 500.00,
                    MinPrice = 90.00,
                };
            }

             
            //Fazer a logica para buscar ou do DB ou HTTP/RabbitMQ todos os dados das criptos e guardar no mongoDB  ou SQL para armazenar
            //E deixar banco de dadps de histórico para quando for fazer as estatisticas, já ter o dado no nosso banco.
        }
    }
}
