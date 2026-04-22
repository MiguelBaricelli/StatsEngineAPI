using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Models.NewsService
{
    public class MarketSentimentResult
    {
        public string Sentiment { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int Count { get; set; }
    }
}
