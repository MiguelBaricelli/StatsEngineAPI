using Microsoft.Extensions.Configuration;
using StatsEngineAPI.Domain.Models;
using StatsEngineAPI.Domain.Models.NewsService;
using StatsEngineAPI.Infrastructure.MarketDataCentralizer;
using StatsEngineAPI.Infrastructure.Repository.B3;


namespace StatsEngineAPI.Application.Services.Price
{
    public class PriceService 
    {
        private readonly HttpClient _httpClient;
        private readonly AlphaVantageMarketNewsIntegration _alphaVantageMarketNewsIntegration;
        private readonly BrApiIntegration _brApiIntegration;
        private readonly IMarketDataCentralizerIntegration _marketDataCentralizer;

        // Sufixos e padrões que indicam ativo B3
        private static readonly string[] B3Exchanges = { ".SA", ".F" };
        private static readonly int[] B3TickerLengths = { 5, 6 }; // PETR4, VALE3, MGLU3

        public PriceService(HttpClient httpClient, IConfiguration configuration, 
            AlphaVantageMarketNewsIntegration alphaVantageMarketNewsIntegration,
            BrApiIntegration brApiIntegration,
            IMarketDataCentralizerIntegration marketDataCentralizer)
        {
            _httpClient = httpClient;
            _alphaVantageMarketNewsIntegration = alphaVantageMarketNewsIntegration;
            _brApiIntegration = brApiIntegration;
            _marketDataCentralizer = marketDataCentralizer;
        }

        public async Task<AssetQuoteUniq?> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return null;
                
            symbol = symbol.Trim().ToUpper();

            return IsB3Asset(symbol)
                ? _marketDataCentralizer.GetIbovespaData(symbol, cancellationToken).Result.Results.ToList().Select(r => new AssetQuoteUniq
                {
                    Symbol = r.Symbol,
                    Name = r.LongName.Length > 0 ? r.LongName : r.ShortName,
                    Market = "B3",
                    Price = r.RegularMarketPrice,
                    Change = r.RegularMarketChange,
                    ChangePercent = r.RegularMarketChangePercent,
                    DayHigh = r.RegularMarketDayHigh,
                    DayLow = r.RegularMarketDayLow,
                    Volume = r.RegularMarketVolume,
                    Currency = "BRL"
                }).FirstOrDefault()
                : await _alphaVantageMarketNewsIntegration.GetAlphaVantageQuoteAsync(symbol, cancellationToken);
        }

        // ================================
        // DETECÇÃO DE MERCADO
        // ================================

        private static bool IsB3Asset(string symbol)
        {
            if (B3Exchanges.Any(s => symbol.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                return true;

            // B3: 5-6 chars, termina com número (PETR4, VALE3, BBDC4, MGLU3)
            return B3TickerLengths.Contains(symbol.Length)
                && symbol.All(char.IsLetterOrDigit)
                && char.IsDigit(symbol[^1]); // ← último char é número
        }




    }
}