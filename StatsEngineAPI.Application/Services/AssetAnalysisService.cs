using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Application.Services.AssetAnalysis;
using StatsEngineAPI.Application.Services.Price;
using StatsEngineAPI.Domain.Interfaces.Infra.MarketNews;
using StatsEngineAPI.Domain.Models;

namespace StatsEngineAPI.Application.Services
{
    public class AssetAnalysisService
    {
        private readonly IAlphaVantageMarketNewsIntegration _newsConsumer;
        private readonly PriceService _priceService;
        private readonly AssetScoringService _scoringService;
        private readonly ParseDataHelper _parseDataHelper;

        public AssetAnalysisService(
            IAlphaVantageMarketNewsIntegration newsConsumer,
            PriceService priceService,
            AssetScoringService scoringService,
            ParseDataHelper parseDataHelper)
        {
            _newsConsumer = newsConsumer;
            _priceService = priceService;
            _scoringService = scoringService;
            _parseDataHelper = parseDataHelper;
        }

        /// <summary>
        /// Análise completa do ativo: preço atual + notícias filtradas + score + condição de mercado.
        /// </summary>
        public async Task<AssetAnalysisResult> AnalyzeAssetAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            // Executa preço e notícias em paralelo para melhor performance
            var priceTask = _priceService.GetQuoteAsync(symbol, cancellationToken);
            var newsTask = _newsConsumer.GetLatestNewsAsync(symbol, cancellationToken);

            await Task.WhenAll(priceTask, newsTask);

            var quote = await priceTask;
            var newsResp = await newsTask;

            // ── Monta preço ───────────────────────────────────────────
            var priceInfo = quote != null
                ? new AssetPriceInfo
                {
                    Current = quote.Price,
                    Change = quote.Change,
                    ChangePercent = quote.ChangePercent,
                    DayHigh = quote.DayHigh,
                    DayLow = quote.DayLow,
                    Volume = quote.Volume,
                    Currency = quote.Currency
                }
                : new AssetPriceInfo();

            // ── Processa notícias ─────────────────────────────────────
            var feed = newsResp?.Feed ?? new List<Domain.Models.MarketNews.FeedItem>();

            var relevantNews = _scoringService.GetRelevantNews(symbol, feed, top: 5);
            var assetScore = _scoringService.CalculateAssetScore(symbol, feed);
            var condition = _scoringService.GetMarketCondition(assetScore, relevantNews);

            return new AssetAnalysisResult
            {
                Symbol = symbol.ToUpper(),
                AssetName = quote?.Name ?? symbol.ToUpper(),
                Market = quote?.Market ?? DetectMarket(symbol),
                AnalysisTimestamp = DateTime.UtcNow,
                Price = priceInfo,
                AssetScore = assetScore,
                MarketCondition = condition,
                RelevantNews = relevantNews
            };
        }

        private static string DetectMarket(string symbol)
        {
            var s = symbol.ToUpper();
            if (s.EndsWith(".SA")) return "B3";
            if (s.Length is 5 or 6 && s.All(char.IsLetterOrDigit)) return "B3";
            return "NYSE/NASDAQ";
        }
    }
}