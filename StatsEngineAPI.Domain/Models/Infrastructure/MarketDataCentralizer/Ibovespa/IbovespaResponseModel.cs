using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer.Ibovespa
{
    public class IbovespaResponseModel
    {
        public string Symbol { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Currency { get; set; }
        public decimal RegularMarketPrice { get; set; }
        public decimal RegularMarketDayHigh { get; set; }
        public decimal RegularMarketDayLow { get; set; }
        public string RegularMarketDayRange { get; set; }
        public decimal RegularMarketChange { get; set; }
        public decimal RegularMarketChangePercent { get; set; }
        public DateTime RegularMarketTime { get; set; }
        public long MarketCap { get; set; }
        public long RegularMarketVolume { get; set; }
        public decimal RegularMarketPreviousClose { get; set; }
        public decimal RegularMarketOpen { get; set; }
        public string FiftyTwoWeekRange { get; set; }
        public decimal FiftyTwoWeekLow { get; set; }
        public decimal FiftyTwoWeekHigh { get; set; }
        public decimal PriceEarnings { get; set; }
        public decimal EarningsPerShare { get; set; }
        public string LogoUrl { get; set; }
    }

}
