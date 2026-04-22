using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Interfaces.Infra.MarketNews;
using StatsEngineAPI.Domain.Models.MarketNews;
using StatsEngineAPI.Domain.Models.NewsService;
using System.Globalization;

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

        // ================================
        // MÉTODOS EXISTENTES
        // ================================

        public async Task<NewsFeedResponse?> GetLatestNewsAsync(string symbol, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(symbol, cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        private static readonly string[] ImportantTickers =
            { "SPX", "USD", "BRL", "EUR", "OIL", "GOLD" };

        /// <summary>
        /// Retorna a última notícia com maior relevância e score.
        /// </summary>
        public async Task<ProcessedNews> GetBestTradeNewsAsync(CancellationToken cancellationToken)
        {
            var response = await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(null, cancellationToken);

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
        /// Retorna lista das notícias mais relevantes até as menos relevantes.
        /// </summary>
        public async Task<List<ProcessedNews>> GetRankedTradeNews(
            CancellationToken cancellationToken,
            int top = 10)
        {
            var response = await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(null, cancellationToken);

            if (response == null || response.Feed == null || !response.Feed.Any())
                return new List<ProcessedNews>();

            var filtered = FilterRecentNews(response.Feed, 2);
            filtered = FilterRelevantTickers(filtered);
            filtered = RemoveNeutralNews(filtered);

            if (!filtered.Any())
                filtered = response.Feed;

            return filtered
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
        }

        // ================================
        // NOVOS MÉTODOS — MARKET INTELLIGENCE
        // ================================

        /// <summary>
        /// 1. Market Sentiment
        /// Calcula o sentimento médio do mercado com base nas notícias recentes.
        /// Retorna se o mercado está Bullish, Bearish ou Neutral.
        /// </summary>
        public async Task<MarketSentimentResult> GetMarketSentimentAsync(CancellationToken cancellationToken)
        {
            var response = await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(null, cancellationToken);

            if (response?.Feed == null || !response.Feed.Any())
                return new MarketSentimentResult { Sentiment = "Neutral", Score = 0, Count = 0 };

            var recentNews = FilterRecentNews(response.Feed, 2);

            // Se não há notícias recentes, usa o feed completo como fallback
            var feed = recentNews.Any() ? recentNews : response.Feed;

            var avg = feed.Average(n => n.OverallSentimentScore);

            var sentiment = avg switch
            {
                > 0.2m => "Bullish",
                < -0.2m => "Bearish",
                _ => "Neutral"
            };

            return new MarketSentimentResult
            {
                Sentiment = sentiment,
                Score = Math.Round(avg, 4),
                Count = feed.Count
            };
        }

        /// <summary>
        /// 2. Market Summary
        /// Gera um resumo inteligente das notícias mais relevantes do mercado.
        /// Utiliza o top N de notícias rankeadas para compor o resumo.
        /// </summary>
        public async Task<MarketSummaryResult> GetMarketSummaryAsync(
            CancellationToken cancellationToken,
            int top = 5)
        {
            var response = await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(null, cancellationToken);

            if (response?.Feed == null || !response.Feed.Any())
                return new MarketSummaryResult { Summary = "Sem notícias disponíveis.", Total = 0 };

            var filtered = FilterRecentNews(response.Feed, 2);
            filtered = FilterRelevantTickers(filtered);
            filtered = RemoveNeutralNews(filtered);

            if (!filtered.Any())
                filtered = response.Feed;

            var topNews = filtered
                .Select(n => new { News = n, Score = CalculateScore(n) })
                .OrderByDescending(x => x.Score)
                .Take(top)
                .Select(x => x.News)
                .ToList();

            var titles = topNews.Select(n => n.Title).ToList();
            var summary = string.Join(" | ", titles);

            return new MarketSummaryResult
            {
                Summary = summary,
                Total = topNews.Count,
                Titles = titles
            };
        }

        /// <summary>
        /// 3. Volatility Alert
        /// Detecta possíveis momentos de alta volatilidade no mercado.
        /// Conta as notícias com impacto absoluto elevado (|SentimentScore| > 0.5).
        /// </summary>
        public async Task<VolatilityAlertResult> GetVolatilityAlertAsync(CancellationToken cancellationToken)
        {
            var response = await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(null, cancellationToken);

            if (response?.Feed == null || !response.Feed.Any())
                return new VolatilityAlertResult
                {
                    Volatility = "UNKNOWN",
                    HighImpactNews = 0,
                    Description = "Sem dados suficientes para avaliar volatilidade."
                };

            var recentFeed = FilterRecentNews(response.Feed, 2);
            var feed = recentFeed.Any() ? recentFeed : response.Feed;

            var highImpactCount = feed.Count(n => Math.Abs(n.OverallSentimentScore) > 0.5m);

            var volatility = highImpactCount switch
            {
                > 5 => "EXTREME",
                > 3 => "HIGH",
                > 1 => "MODERATE",
                _ => "NORMAL"
            };

            var description = volatility switch
            {
                "EXTREME" => "Mercado com volatilidade extrema. Evite operar ou use stops curtos.",
                "HIGH" => "Alta volatilidade detectada. Opere com cautela e gerencie o risco.",
                "MODERATE" => "Volatilidade moderada. Monitore as posições abertas.",
                _ => "Mercado estável. Condições normais para operar."
            };

            return new VolatilityAlertResult
            {
                Volatility = volatility,
                HighImpactNews = highImpactCount,
                Description = description
            };
        }

        /// <summary>
        /// 4. Trade Signals
        /// Gera sinais de BUY, SELL ou HOLD com base no sentimento médio ponderado.
        /// Usa apenas notícias relevantes e recentes para maior precisão.
        /// </summary>
        public async Task<TradeSignalResult> GetTradeSignalAsync(CancellationToken cancellationToken)
        {
            var response = await _alphaVantageMarketNewsConsumer.GetLatestNewsAsync(null, cancellationToken);

            if (response?.Feed == null || !response.Feed.Any())
                return new TradeSignalResult
                {
                    Signal = "HOLD",
                    Sentiment = 0,
                    Reasoning = "Sem dados suficientes para gerar sinal."
                };

            var filtered = FilterRecentNews(response.Feed, 2);
            filtered = FilterRelevantTickers(filtered);

            if (!filtered.Any())
                filtered = response.Feed;

            // Score ponderado: itens mais relevantes têm mais peso no sinal
            var weightedSentiment = filtered
                .Select(n => new
                {
                    Score = n.OverallSentimentScore,
                    Relevance = GetMaxRelevance(n)
                })
                .Where(x => x.Relevance > 0)
                .Select(x => x.Score * x.Relevance)
                .DefaultIfEmpty(0)
                .Average();

            var signal = weightedSentiment switch
            {
                > 0.3m => "BUY",
                < -0.3m => "SELL",
                _ => "HOLD"
            };

            var reasoning = signal switch
            {
                "BUY" => $"Sentimento ponderado positivo ({weightedSentiment:+0.0000;-0.0000}). Notícias relevantes apontam para valorização.",
                "SELL" => $"Sentimento ponderado negativo ({weightedSentiment:+0.0000;-0.0000}). Notícias relevantes apontam para desvalorização.",
                _ => $"Sentimento neutro ({weightedSentiment:+0.0000;-0.0000}). Aguarde melhor oportunidade."
            };

            return new TradeSignalResult
            {
                Signal = signal,
                Sentiment = Math.Round(weightedSentiment, 4),
                Reasoning = reasoning
            };
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
        // REGIÃO
        // ================================

        public string GetRegion(FeedItem item)
        {
            var tickers = item.TickerSentiment.Select(t => t.Ticker).ToList();

            if (tickers.Any(t => t.Contains("BRL"))) return "Brasil";
            if (tickers.Any(t => t.Contains("USD") || t.Contains("SPX"))) return "Estados Unidos";
            if (tickers.Any(t => t.Contains("EUR"))) return "Europa";
            if (tickers.Any(t => t.Contains("OIL"))) return "Commodities";

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