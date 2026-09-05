using Microsoft.Extensions.Logging;
using StatsEngineAPI.Domain.Interfaces.Infra.Cripto;
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

        private readonly ICriptoHistoryRepository _criptoHistoryRepository;
        public CollectionDailyMarketDataService(ILogger<CollectionDailyMarketDataService> logger,
            ICriptoHistoryRepository criptoHistoryRepository)
        {
            _logger = logger;
            _criptoHistoryRepository = criptoHistoryRepository;
        }

        public async Task CollectDailyDataAsync(CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();
            try
            {
          
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

                try
                {
                    await _criptoHistoryRepository.InsertManyAsync(mockData);
                }
                catch (Exception ex)
                {
                    _logger.LogError("[{Class}] [{Method}] Erro ao inserir histórico de cripto no mongoDb. Date: {Data}, Tempo: {Time}",
                        nameof(CollectionDailyMarketDataService), nameof(CollectDailyDataAsync), DateTime.Now, stopWatch.ElapsedMilliseconds);
                    throw;
                }


                _logger.LogInformation("[{Class}] [{Method}] Dados inseridos com sucesso no mongoDb. Date: {Data}, Tempo: {Time}",
                        nameof(CollectionDailyMarketDataService), nameof(CollectDailyDataAsync), DateTime.Now, stopWatch.ElapsedMilliseconds);

            } 
            catch (Exception ex)
            {
                _logger.LogError("[{Class}] [{Method}] Erro ao coletar dados de histórico de cripto do worker. Date: {Data}",
                        nameof(CollectionDailyMarketDataService), nameof(CollectDailyDataAsync), DateTime.Now);
                throw;
            }
            finally
            {
                stopWatch.Stop();
            }
        }
    }
}
