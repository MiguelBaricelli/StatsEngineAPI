using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Infrastructure.MarketDataCentralizer.Config
{
    public class MarketDataCentralizerConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeOut { get; set; }
    }
}
