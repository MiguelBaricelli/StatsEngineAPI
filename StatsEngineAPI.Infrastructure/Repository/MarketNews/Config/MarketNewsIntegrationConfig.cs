using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Infrastructure.Repository.MarketNews.Config
{
    public class MarketNewsIntegrationConfig
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int Timeout { get; set; }
    }
}
