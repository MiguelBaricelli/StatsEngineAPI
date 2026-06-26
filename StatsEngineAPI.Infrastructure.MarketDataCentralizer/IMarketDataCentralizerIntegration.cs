using StatsEngineAPI.Domain.Dividends;
using StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer;
using StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer.Ibovespa;
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
        Task<IbovespaApiResponse> GetIbovespaData(string symbol, CancellationToken cancellationToken);
    }
}
