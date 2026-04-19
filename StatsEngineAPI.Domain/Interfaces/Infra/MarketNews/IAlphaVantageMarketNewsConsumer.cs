using StatsEngineAPI.Domain.Models.MarketNews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Interfaces.Infra.MarketNews
{
    public interface IAlphaVantageMarketNewsConsumer
    {
        Task<NewsFeedResponse?> GetLatestNewsAsync(string symbol, CancellationToken cancellationToken = default);
    }
}
