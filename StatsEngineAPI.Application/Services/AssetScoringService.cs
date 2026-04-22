using StatsEngineAPI.Application.Helpers;
using StatsEngineAPI.Domain.Models;
using StatsEngineAPI.Domain.Models.MarketNews;
using System.Globalization;

namespace StatsEngineAPI.Application.Services.AssetAnalysis
{
    public class AssetScoringService
    {
        private readonly ParseDataHelper _parseDataHelper;

        // Mapeamento de tickers de notícia para termos relacionados ao ativo
        // Permite correlacionar "OIL" com PETR4, "USD" com ativos exportadores, etc.

        //Futuramente colocar via DB ou endpoint de cadastro para adicionar e alterar ativos a hora que precisar**
        private static readonly Dictionary<string, string[]> AssetNewsCorrelation = new(StringComparer.OrdinalIgnoreCase)
        {
            // B3
            { "PETR4", new[] { "OIL", "PETR", "PETROBRAS", "BRL", "ENERGY" } },
            { "PETR3", new[] { "OIL", "PETR", "PETROBRAS", "BRL", "ENERGY" } },
            { "VALE3", new[] { "IRON", "VALE", "MINING", "BRL", "CHINA"    } },
            { "ITUB4", new[] { "BRL", "FINANCE", "BANK", "BRAZIL"           } },
            { "BBDC4", new[] { "BRL", "FINANCE", "BANK", "BRAZIL"           } },
            { "MGLU3", new[] { "BRL", "RETAIL", "BRAZIL"                    } },
            // Americanos
            { "AAPL",  new[] { "TECHNOLOGY", "USD", "SPX", "TECH"           } },
            { "MSFT",  new[] { "TECHNOLOGY", "USD", "SPX", "TECH", "AI"     } },
            { "NVDA",  new[] { "TECHNOLOGY", "USD", "SPX", "AI", "CHIP"     } },
            { "TSLA",  new[] { "EV", "USD", "SPX", "ENERGY", "AUTO"        } },
            { "AMZN",  new[] { "RETAIL", "USD", "SPX", "TECH", "CLOUD"     } },
            // Forex / Commodities
            { "USD",   new[] { "USD", "SPX", "FINANCE"                      } },
            { "BRL",   new[] { "BRL", "BRAZIL", "FINANCE"                   } },
            { "GOLD",  new[] { "GOLD", "COMMODITY", "USD"                   } },
            { "OIL",   new[] { "OIL", "ENERGY", "OPEC", "USD"              } },
        };

        public AssetScoringService(ParseDataHelper parseDataHelper)
        {
            _parseDataHelper = parseDataHelper;
        }

        /// <summary>
        /// Filtra e ranqueia notícias relevantes para o ativo especificado.
        /// </summary>
        public List<RelevantNewsItem> GetRelevantNews(string symbol, List<FeedItem> feed, int top = 5)
        {
            var correlatedTerms = GetCorrelationTerms(symbol);

            return feed
                .Where(n => IsRelevantForAsset(n, symbol, correlatedTerms))
                .Select(n => new
                {
                    Item = n,
                    Relevance = CalculateAssetRelevance(n, symbol, correlatedTerms),
                    Date = _parseDataHelper.ParseDateByAlphaVantage(n.TimePublished)
                })
                .Where(x => x.Relevance > 0)
                .OrderByDescending(x => x.Relevance)
                .ThenByDescending(x => x.Date)
                .Take(top)
                .Select(x => new RelevantNewsItem
                {
                    Title = x.Item.Title,
                    Source = x.Item.Source,
                    PublishedAt = x.Date,
                    Sentiment = x.Item.OverallSentimentLabel,
                    RelevanceScore = Math.Round(x.Relevance, 4),
                    ImpactOnAsset = MapImpact(x.Item.OverallSentimentScore),
                    Url = x.Item.Url
                })
                .ToList();
        }

        /// <summary>
        /// Calcula o score consolidado do ativo com base nas notícias relevantes.
        /// </summary>
        public AssetScoreInfo CalculateAssetScore(string symbol, List<FeedItem> feed)
        {
            var correlatedTerms = GetCorrelationTerms(symbol);

            var relevantItems = feed
                .Where(n => IsRelevantForAsset(n, symbol, correlatedTerms))
                .Select(n => new
                {
                    SentimentScore = n.OverallSentimentScore,
                    Relevance = CalculateAssetRelevance(n, symbol, correlatedTerms)
                })
                .Where(x => x.Relevance > 0)
                .ToList();

            if (!relevantItems.Any())
                return new AssetScoreInfo
                {
                    Value = 0,
                    Classification = "NO_DATA",
                    Direction = "NEUTRAL"
                };

            // Score ponderado pela relevância
            var totalRelevance = relevantItems.Sum(x => x.Relevance);
            var weightedScore = relevantItems.Sum(x => x.SentimentScore * x.Relevance) / totalRelevance;
            var absoluteScore = (decimal)Math.Abs((double)weightedScore);

            var direction = weightedScore switch
            {
                > 0.15m => "BULLISH",
                < -0.15m => "BEARISH",
                _ => "NEUTRAL"
            };

            var classification = absoluteScore switch
            {
                >= 0.7m => "CRITICAL_IMPACT",
                >= 0.4m => "HIGH_IMPACT",
                >= 0.2m => "MEDIUM_IMPACT",
                _ => "LOW_IMPACT"
            };

            return new AssetScoreInfo
            {
                Value = Math.Round(absoluteScore, 4),
                Classification = classification,
                Direction = direction
            };
        }

        /// <summary>
        /// Determina a condição de mercado e recomendação para o ativo.
        /// Combina volatilidade das notícias com direção do score.
        /// </summary>
        public MarketConditionInfo GetMarketCondition(AssetScoreInfo score, List<RelevantNewsItem> news)
        {
            var highImpactCount = news.Count(n => n.RelevanceScore > 0.5m);
            var negativeCount = news.Count(n => n.ImpactOnAsset == "NEGATIVE");
            var positiveCount = news.Count(n => n.ImpactOnAsset == "POSITIVE");

            // Determina status
            string status = (highImpactCount, score.Value) switch
            {
                ( > 4, >= 0.6m) => "CRITICAL",
                ( > 2, >= 0.4m) => "VOLATILE",
                (_, >= 0.2m) => "FAVORABLE",
                _ => "STABLE"
            };

            // Ajusta para desfavorável se a direção é bearish com alta volatilidade
            if (status == "FAVORABLE" && score.Direction == "BEARISH")
                status = "VOLATILE";

            var description = status switch
            {
                "CRITICAL" => "Condição crítica. Múltiplas notícias de alto impacto. Evite novas posições.",
                "VOLATILE" => "Alta volatilidade detectada. Opere com cautela e stops curtos.",
                "FAVORABLE" => score.Direction == "BULLISH"
                                ? "Condição favorável para compra. Notícias positivas dominam."
                                : "Condição favorável para venda. Notícias negativas dominam.",
                _ => "Mercado estável. Sem eventos de alto impacto no radar."
            };

            // Recomendação final
            string recommendation = (status, score.Direction) switch
            {
                ("CRITICAL", _) => "WAIT",
                ("VOLATILE", "BEARISH") => "SELL",
                ("VOLATILE", "BULLISH") => "WAIT",
                ("FAVORABLE", "BULLISH") => "BUY",
                ("FAVORABLE", "BEARISH") => "SELL",
                _ => "HOLD"
            };

            return new MarketConditionInfo
            {
                Status = status,
                Description = description,
                Recommendation = recommendation
            };
        }

        // ================================
        // HELPERS PRIVADOS
        // ================================

        private string[] GetCorrelationTerms(string symbol)
        {
            var cleanSymbol = symbol.Replace(".SA", "").ToUpper();
            return AssetNewsCorrelation.TryGetValue(cleanSymbol, out var terms)
                ? terms
                : new[] { cleanSymbol };
        }

        private static bool IsRelevantForAsset(FeedItem item, string symbol, string[] correlationTerms)
        {
            var cleanSymbol = symbol.Replace(".SA", "").ToUpper();

            // Direto: ticker aparece nas notícias
            var directMatch = item.TickerSentiment.Any(t =>
                t.Ticker.Contains(cleanSymbol, StringComparison.OrdinalIgnoreCase));

            // Indireto: termos correlacionados
            var indirectMatch = item.TickerSentiment.Any(t =>
                correlationTerms.Any(term =>
                    t.Ticker.Contains(term, StringComparison.OrdinalIgnoreCase)));

            // Por título
            var titleMatch = correlationTerms.Any(term =>
                item.Title.Contains(term, StringComparison.OrdinalIgnoreCase));

            return directMatch || indirectMatch || titleMatch;
        }

        private static decimal CalculateAssetRelevance(FeedItem item, string symbol, string[] correlationTerms)
        {
            var cleanSymbol = symbol.Replace(".SA", "").ToUpper();
            decimal maxRelevance = 0;

            foreach (var ticker in item.TickerSentiment)
            {
                decimal.TryParse(ticker.RelevanceScore, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var relevance);

                // Match direto tem peso total
                if (ticker.Ticker.Contains(cleanSymbol, StringComparison.OrdinalIgnoreCase))
                    maxRelevance = Math.Max(maxRelevance, relevance);
                // Match indireto tem 60% do peso
                else if (correlationTerms.Any(t => ticker.Ticker.Contains(t, StringComparison.OrdinalIgnoreCase)))
                    maxRelevance = Math.Max(maxRelevance, relevance * 0.6m);
            }

            return maxRelevance;
        }

        private static string MapImpact(decimal sentimentScore) => sentimentScore switch
        {
            > 0.1m => "POSITIVE",
            < -0.1m => "NEGATIVE",
            _ => "NEUTRAL"
        };
    }
}
