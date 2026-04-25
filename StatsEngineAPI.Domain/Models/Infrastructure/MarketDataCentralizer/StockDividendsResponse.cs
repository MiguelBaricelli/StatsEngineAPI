namespace StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer
{
    public class StockDividendResponse
    {
       
        public string Symbol { get; set; }

        public List<DividendEntry> Data { get; set; }
    }

    public class DividendEntry
    {
       
        public string ExDividendDate { get; set; }

     
        public string DeclarationDate { get; set; }

        public string RecordDate { get; set; }

        public string PaymentDate { get; set; }

      
        public string Amount { get; set; }
    }
}
