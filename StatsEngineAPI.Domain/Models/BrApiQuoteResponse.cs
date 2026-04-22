using System.Text.Json.Serialization;

namespace StatsEngineAPI.Domain.Models
{
    public class BrApiQuoteResponse
    {
        public List<BrApiQuoteResult>? Results { get; set; }
    }

    public class BrApiQuoteResult
    {
        public string Symbol { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string LongName { get; set; } = string.Empty;
        public decimal RegularMarketPrice { get; set; }
        public decimal RegularMarketChange { get; set; }
        public decimal RegularMarketChangePercent { get; set; }
        public decimal RegularMarketDayHigh { get; set; }
        public decimal RegularMarketDayLow { get; set; }
        public long RegularMarketVolume { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
    }
}