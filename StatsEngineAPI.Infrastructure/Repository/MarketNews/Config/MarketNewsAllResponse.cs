using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StatsEngineAPI.Infrastructure.Repository.MarketNews.Models
{
    public class MarketNewsAllResponse
    {
        [JsonPropertyName("meta")]
        public MarketNewsAllMeta? Meta { get; set; }

        [JsonPropertyName("data")]
        public List<NewsArticle> Data { get; set; } = new();
    }

    public class MarketNewsAllMeta
    {
        [JsonPropertyName("found")]
        public int Found { get; set; }

        [JsonPropertyName("returned")]
        public int Returned { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }
    }

    // ── Artigo de notícia ──────────────────────────────────────────────────────
    public class NewsArticle
    {
        /// <summary>ID único do artigo — use para buscar via /news/uuid/{uuid}</summary>
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("keywords")]
        public string? Keywords { get; set; }

        /// <summary>Trecho do corpo do artigo</summary>
        [JsonPropertyName("snippet")]
        public string? Snippet { get; set; }

        /// <summary>URL original da notícia</summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        /// <summary>Data/hora de publicação (UTC)</summary>
        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        /// <summary>Score de relevância — preenchido só quando search é usado</summary>
        [JsonPropertyName("relevance_score")]
        public double? RelevanceScore { get; set; }

        /// <summary>Ações identificadas na notícia com seus sentimentos</summary>
        [JsonPropertyName("entities")]
        public List<NewsEntity> Entities { get; set; } = new();

        /// <summary>Artigos similares agrupados</summary>
        [JsonPropertyName("similar")]
        public List<NewsArticle> Similar { get; set; } = new();
    }

    // ── Entidade (ação) identificada ───────────────────────────────────────────
    public class NewsEntity
    {
        /// <summary>Ticker da ação. Ex: "TSLA"</summary>
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("exchange")]
        public string? Exchange { get; set; }

        [JsonPropertyName("exchange_long")]
        public string? ExchangeLong { get; set; }

        /// <summary>País da exchange. Ex: "us"</summary>
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        /// <summary>Tipo. Ex: "equity", "cryptocurrency", "index"</summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>Setor. Ex: "Technology", "Consumer Cyclical"</summary>
        [JsonPropertyName("industry")]
        public string? Industry { get; set; }

        /// <summary>Força do match entre artigo e entidade (quanto maior, mais relevante)</summary>
        [JsonPropertyName("match_score")]
        public double MatchScore { get; set; }

        /// <summary>
        /// Sentimento médio: -1 (muito negativo) a +1 (muito positivo). 0 = neutro.
        /// </summary>
        [JsonPropertyName("sentiment_score")]
        public double SentimentScore { get; set; }

        [JsonPropertyName("highlights")]
        public List<NewsHighlight> Highlights { get; set; } = new();
    }

    // ── Trecho destacado dentro do artigo ─────────────────────────────────────
    public class NewsHighlight
    {
        /// <summary>Trecho do texto onde a ação foi mencionada</summary>
        [JsonPropertyName("highlight")]
        public string? Highlight { get; set; }

        /// <summary>Sentimento deste trecho específico</summary>
        [JsonPropertyName("sentiment")]
        public double Sentiment { get; set; }

        /// <summary>"title" ou "main_text"</summary>
        [JsonPropertyName("highlighted_in")]
        public string? HighlightedIn { get; set; }
    }
}