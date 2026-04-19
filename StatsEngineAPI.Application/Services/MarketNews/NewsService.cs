using StatsEngineAPI.Domain.Interfaces.Infra.MarketNews;
using StatsEngineAPI.Domain.Models.MarketNews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Services.MarketNews
{
    public class NewsService
    {
        private readonly IAlphaVantageMarketNewsConsumer _alphaVantageMarketNewsConsumer;

        public NewsService(IAlphaVantageMarketNewsConsumer alphaVantageMarketNewsConsumer)
        {
            _alphaVantageMarketNewsConsumer = alphaVantageMarketNewsConsumer;
        }

        public async Task<NewsFeedResponse?> GetLatestNewsAsync(string symbol, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(symbol, cancellationToken);

            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
