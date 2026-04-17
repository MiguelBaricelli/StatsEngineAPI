namespace StatsEngineAPI.Domain.MarketNews
{
    /// <summary>
    /// Parâmetros para GET /v1/news/all (disponível no plano free).
    /// Retorna: notícia completa + data + sentimento + ações impactadas.
    /// </summary>
    public class MarketNewsQueryParams
    {
        // ── Filtro de ações ────────────────────────────────────────────────
        /// <summary>Tickers separados por vírgula. Ex: "TSLA,AAPL,AMZN"</summary>
        public string? Symbols { get; set; }

        /// <summary>
        /// Se true, retorna no array entities APENAS as ações do filtro symbols.
        /// Recomendado: true — evita ruído.
        /// </summary>
        public bool? FilterEntities { get; set; } = true;

        /// <summary>
        /// Se true, garante que toda notícia retornada tem ao menos uma ação identificada.
        /// Recomendado: true — remove artigos sem contexto financeiro.
        /// </summary>
        public bool? MustHaveEntities { get; set; } = true;

        // ── Filtros de sentimento ──────────────────────────────────────────
        /// <summary>Sentiment >= x. Ex: 0.1 = só positivos. Range: -1 a +1.</summary>
        public double? SentimentGte { get; set; }

        /// <summary>Sentiment <= x. Ex: -0.1 = só negativos. Range: -1 a +1.</summary>
        public double? SentimentLte { get; set; }

        // ── Filtros de data ────────────────────────────────────────────────
        /// <summary>Data exata. Formato: yyyy-MM-dd. Ex: "2026-04-16"</summary>
        public string? PublishedOn { get; set; }

        /// <summary>Artigos publicados após esta data. Formato: yyyy-MM-ddTHH:mm:ss</summary>
        public string? PublishedAfter { get; set; }

        /// <summary>Artigos publicados antes desta data. Formato: yyyy-MM-ddTHH:mm:ss</summary>
        public string? PublishedBefore { get; set; }

        // ── Filtros adicionais ─────────────────────────────────────────────
        /// <summary>Tipos de entidade. Ex: "equity,index"</summary>
        public string? EntityTypes { get; set; }

        /// <summary>Indústrias. Ex: "Technology,Healthcare"</summary>
        public string? Industries { get; set; }

        /// <summary>Países da exchange. Ex: "us,ca"</summary>
        public string? Countries { get; set; }

        /// <summary>Idioma. Ex: "en" ou "en,es"</summary>
        public string? Language { get; set; } = "en";

        /// <summary>Busca textual no título e corpo. Ex: "ipo -nyse"</summary>
        public string? Search { get; set; }

        /// <summary>Agrupa artigos similares para evitar duplicatas. Default: true</summary>
        public bool? GroupSimilar { get; set; } = true;

        // ── Ordenação ─────────────────────────────────────────────────────
        /// <summary>
        /// Campo de ordenação.
        /// Opções: published_at | entity_match_score | entity_sentiment_score | relevance_score
        /// </summary>
        public string? Sort { get; set; } = "entity_sentiment_score";

        /// <summary>Direção: "desc" ou "asc"</summary>
        public string? SortOrder { get; set; } = "desc";

        // ── Paginação ──────────────────────────────────────────────────────
        public int? Limit { get; set; }
        public int? Page { get; set; }
    }
}