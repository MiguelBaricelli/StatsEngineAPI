namespace StatsEngineAPI.Domain.Models
{
    public class AssetAnalysisResult
    {
        public string Symbol { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Market { get; set; } = string.Empty; // "B3" | "NYSE" | "NASDAQ"
        public DateTime AnalysisTimestamp { get; set; } = DateTime.UtcNow;

        public AssetPriceInfo Price { get; set; } = new();
        public MarketConditionInfo MarketCondition { get; set; } = new();
        public AssetScoreInfo AssetScore { get; set; } = new();
        public List<RelevantNewsItem> RelevantNews { get; set; } = new();
    }

    public class AssetPriceInfo
    {
        public decimal Current { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal Volume { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    public class MarketConditionInfo
    {
        /// <summary>FAVORABLE | STABLE | VOLATILE | CRITICAL</summary>
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        /// <summary>BUY | SELL | HOLD | WAIT</summary>
        public string Recommendation { get; set; } = string.Empty;
    }

    public class AssetScoreInfo
    {
        /// <summary>Score de 0 a 1 baseado no impacto das notícias no ativo</summary>
        public decimal Value { get; set; }
        /// <summary>LOW_IMPACT | MEDIUM_IMPACT | HIGH_IMPACT | CRITICAL_IMPACT</summary>
        public string Classification { get; set; } = string.Empty;
        /// <summary>BULLISH | BEARISH | NEUTRAL</summary>
        public string Direction { get; set; } = string.Empty;
    }

    public class RelevantNewsItem
    {
        public string Title { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public string Sentiment { get; set; } = string.Empty;
        public decimal RelevanceScore { get; set; }
        /// <summary>POSITIVE | NEGATIVE | NEUTRAL</summary>
        public string ImpactOnAsset { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

}
