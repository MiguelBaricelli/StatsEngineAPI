using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Interfaces.Infra.MarketNews;
using StatsEngineAPI.Domain.Models.MarketNews;
using StatsEngineAPI.Domain.Models.NewsService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Services.MarketNews
{
    public class NewsService
    {
        private readonly IAlphaVantageMarketNewsIntegration _alphaVantageMarketNewsConsumer;
        private readonly ParseDataHelper _parseDataHelper;

        public NewsService(IAlphaVantageMarketNewsIntegration alphaVantageMarketNewsConsumer, ParseDataHelper parseDataHelper)
        {
            _alphaVantageMarketNewsConsumer = alphaVantageMarketNewsConsumer;
            _parseDataHelper = parseDataHelper;
        }

        public async Task<NewsFeedResponse?> GetLatestNewsAsync(string symbol, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(symbol, cancellationToken);

            }
            catch (Exception e)
            {
                throw;
            }

        }

        private static readonly string[] ImportantTickers =
        { "SPX", "USD", "BRL", "EUR", "OIL", "GOLD" };
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Ultima notícia co maior relevancia</returns>
        public async Task<ProcessedNews> GetBestTradeNewsAsync(CancellationToken cancellationToken)
        {
        
        var response = await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(null,cancellationToken);

            if (response?.Feed == null || response == null)
                return null;

            var filtered = FilterRecentNews(response.Feed, 2);
            filtered = FilterRelevantTickers(filtered);
            filtered = RemoveNeutralNews(filtered);

            if (!filtered.Any())
                filtered = response.Feed;

            var best = filtered
                .Select(n => new
                {
                    News = n,
                    Score = CalculateScore(n),
                    Date = _parseDataHelper.ParseDateByAlphaVantage(n.TimePublished),
                    Region = GetRegion(n),
                    Summary = BuildSummary(n)
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Date)
                .FirstOrDefault();

            if (best == null) return null;

            return new ProcessedNews
            {
                Title = best.News.Title,
                Summary = best.Summary,
                Url = best.News.Url,
                Source = best.News.Source,
                Region = best.Region,
                Score = best.Score,
                Sentiment = best.News.OverallSentimentLabel,
                PublishedAt = best.Date
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="top"></param>
        /// <returns>Lista das noticias mais relevantes até as menos relevantes</returns>
        public async Task<List<ProcessedNews>> GetRankedTradeNews(
        CancellationToken cancellationToken,
        int top = 10)
        {
            var response = await _alphaVantageMarketNewsConsumer
                .GetLatestNewsAsync(null, cancellationToken);

            if (response == null || response.Feed == null || !response.Feed.Any())
                return new List<ProcessedNews>();

            var filtered = FilterRecentNews(response.Feed, 2);
            filtered = FilterRelevantTickers(filtered);
            filtered = RemoveNeutralNews(filtered);

            if (!filtered.Any())
                filtered = response.Feed;

            var ranked = filtered
                .Select(n => new
                {
                    News = n,
                    Score = CalculateScore(n),
                    Date = _parseDataHelper.ParseDateByAlphaVantage(n.TimePublished),
                    Region = GetRegion(n),
                    Summary = BuildSummary(n)
                })
                .OrderByDescending(x => x.Score)     
                .ThenByDescending(x => x.Date)       
                .Take(top)                           
                .Select(x => new ProcessedNews
                {
                    Title = x.News.Title,
                    Summary = x.Summary,
                    Url = x.News.Url,
                    Source = x.News.Source,
                    Region = x.Region,
                    Score = x.Score,
                    Sentiment = x.News.OverallSentimentLabel,
                    PublishedAt = x.Date
                })
                .ToList();

            return ranked;
        }

        // ================================
        // FILTROS
        // ================================
        public List<FeedItem> FilterRecentNews(List<FeedItem> feed, int hours)
        {
            var now = DateTime.UtcNow;

            return feed.Where(n =>
            {
                var date = _parseDataHelper.ParseDateByAlphaVantage(n.TimePublished);
                return (now - date).TotalHours <= hours;
            }).ToList();
        }

        public List<FeedItem> FilterRelevantTickers(List<FeedItem> feed)
        {
            return feed.Where(n =>
                n.TickerSentiment.Any(t => ImportantTickers.Contains(t.Ticker))
            ).ToList();
        }

        public List<FeedItem> RemoveNeutralNews(List<FeedItem> feed)
        {
            return feed.Where(n =>
                !string.Equals(n.OverallSentimentLabel, "Neutral", StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        // ================================
        // SCORE INTELIGENTE
        // ================================
        public decimal CalculateScore(FeedItem item)
        {
            var relevance = GetMaxRelevance(item);
            var impact = Math.Abs(item.OverallSentimentScore);

            // Peso maior para relevância
            return (relevance * 0.6m) + (impact * 0.4m);
        }

        public decimal GetMaxRelevance(FeedItem item)
        {
            if (item.TickerSentiment == null || !item.TickerSentiment.Any())
                return 0;

            return item.TickerSentiment
                .Select(t =>
                {
                    decimal.TryParse(t.RelevanceScore, NumberStyles.Any, CultureInfo.InvariantCulture, out var val);
                    return val;
                })
                .Max();
        }

        // ================================
        //  REGIÃO (IMPORTANTE PRA TRADE)
        // ================================
        public string GetRegion(FeedItem item)
        {
            var tickers = item.TickerSentiment.Select(t => t.Ticker).ToList();

            if (tickers.Any(t => t.Contains("BRL")))
                return "Brasil";

            if (tickers.Any(t => t.Contains("USD") || t.Contains("SPX")))
                return "Estados Unidos";

            if (tickers.Any(t => t.Contains("EUR")))
                return "Europa";

            if (tickers.Any(t => t.Contains("OIL")))
                return "Commodities";

            return "Global";
        }

        // ================================
        // SUMMARY INTELIGENTE
        // ================================
        public string BuildSummary(FeedItem item)
        {
            var impact = Math.Abs(item.OverallSentimentScore);

            string intensidade = impact switch
            {
                >= 0.7m => "FORTE",
                >= 0.4m => "MODERADO",
                _ => "FRACO"
            };

            return $"{item.Title} | Impacto: {intensidade} | Sentimento: {item.OverallSentimentLabel}";
        }
    }
}
