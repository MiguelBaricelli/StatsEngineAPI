using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Models.AlphaVantage
{
    public class AlphaVantageQuoteResponse
    {
        [JsonPropertyName("Global Quote")]
        public AlphaVantageGlobalQuote? GlobalQuote { get; set; }
    }

    public class AlphaVantageGlobalQuote
    {
        [JsonPropertyName("01. symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("05. price")]
        public string Price { get; set; } = string.Empty;

        [JsonPropertyName("03. high")]
        public string High { get; set; } = string.Empty;

        [JsonPropertyName("04. low")]
        public string Low { get; set; } = string.Empty;

        [JsonPropertyName("06. volume")]
        public string Volume { get; set; } = string.Empty;

        [JsonPropertyName("09. change")]
        public string Change { get; set; } = string.Empty;

        [JsonPropertyName("10. change percent")]
        public string ChangePercent { get; set; } = string.Empty;
    }
}
