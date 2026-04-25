using StatsEngineAPI.Domain.Dividends;
using StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Infrastructure.MarketDataCentralizer
{
    public interface IMarketDataCentralizerIntegration
    {
        Task<StockDividendResponse> GetDividendsData(string symbol);
    }
}
