using StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer;
using StatsEngineAPI.Infrastructure.MarketDataCentralizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Services.Dividends.MarketDataCentralizer
{
    public class DividendsService 
    {

        private readonly IMarketDataCentralizerIntegration _marketDataCentralizerIntegration;

        public DividendsService(IMarketDataCentralizerIntegration marketDataCentralizerIntegration)
        {
            _marketDataCentralizerIntegration = marketDataCentralizerIntegration;
        }

        public async Task<StockDividendResponse> GetDividendsData(string symbol)
        {
            return await _marketDataCentralizerIntegration.GetDividendsData(symbol);
        }
    }
}
