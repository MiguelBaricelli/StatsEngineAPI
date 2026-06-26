using System.Text.Json.Serialization;

namespace StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer
{
    public class StockDividendResponse
    {
       
        public string Symbol { get; set; }

        public List<DividendEntry> Data { get; set; }
    }

    public class DividendEntry
    {
        [JsonPropertyName("ex_dividend_date")]
        public string ExDividendDate { get; set; }

        [JsonPropertyName("declaration_date")]
        public string DeclarationDate { get; set; }

        [JsonPropertyName("record_date")]
        public string RecordDate { get; set; }

        [JsonPropertyName("payment_date")]
        public string PaymentDate { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }
}
