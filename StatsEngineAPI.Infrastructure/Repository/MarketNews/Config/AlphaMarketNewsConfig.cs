using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Infrastructure.Repository.MarketNews.Config
{
    public class AlphaMarketNewsConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string NewsEndpoint { get; set; } = string.Empty;
        public int Timeout { get; set; } = 30; 
        public string? ApiKey { get; set; }
        public int RetryCount { get; set; } = 4;
        public int InitialRetryDelaySeconds { get; set; } = 1;
    }
}
