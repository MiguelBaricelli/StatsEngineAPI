using StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer.Ibovespa;
using StatsEngineAPI.Infrastructure.MarketDataCentralizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Services.Dividends.MarketDataCentralizer
{
    public class IbovespaService
    {
        private readonly IMarketDataCentralizerIntegration _marketDataCentralizerIntegration;

        public IbovespaService(IMarketDataCentralizerIntegration marketDataCentralizerIntegration)
        {
            _marketDataCentralizerIntegration = marketDataCentralizerIntegration;
        }

        public async Task<IbovespaApiResponse> GetIbovespaData(string symbol, CancellationToken cancellation)
        {
            return await _marketDataCentralizerIntegration.GetIbovespaData(symbol, cancellation);
        }       
    }
}
