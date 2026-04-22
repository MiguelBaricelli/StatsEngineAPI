using Microsoft.Extensions.Configuration;
using StatsEngineAPI.Domain.Models;
using StatsEngineAPI.Domain.Models.NewsService;
using StatsEngineAPI.Infrastructure.Repository.B3;


namespace StatsEngineAPI.Application.Services.Price
{
    public class PriceService 
    {
        private readonly HttpClient _httpClient;
        private readonly AlphaVantageMarketNewsIntegration _alphaVantageMarketNewsIntegration;
        private readonly BrApiIntegration _brApiIntegration;

        // Sufixos e padrões que indicam ativo B3
        private static readonly string[] B3Exchanges = { ".SA", ".F" };
        private static readonly int[] B3TickerLengths = { 5, 6 }; // PETR4, VALE3, MGLU3

        public PriceService(HttpClient httpClient, IConfiguration configuration, 
            AlphaVantageMarketNewsIntegration alphaVantageMarketNewsIntegration,
            BrApiIntegration brApiIntegration)
        {
            _httpClient = httpClient;
            _alphaVantageMarketNewsIntegration = alphaVantageMarketNewsIntegration;
            _brApiIntegration = brApiIntegration;
        }

        public async Task<AssetQuoteUniq?> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return null;

            symbol = symbol.Trim().ToUpper();

            return IsB3Asset(symbol)
                ? await _brApiIntegration.GetBrApiQuoteAsync(symbol, cancellationToken)
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