using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StatsEngineAPI.Domain.MarketNews;
using StatsEngineAPI.Infrastructure.Repository.MarketNews.Config;
using StatsEngineAPI.Infrastructure.Repository.MarketNews.Models;
using System.Text.Json;
using System.Web;

namespace StatsEngineAPI.Infrastructure.Repository.MarketNews
{
    public class MarketNewsIntegration
    {
        private readonly MarketNewsIntegrationConfig _config;
        private readonly ILogger<MarketNewsIntegration> _logger;

        private readonly HttpClient _http;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public MarketNewsIntegration(
            HttpClient httpClient,
            IOptions<MarketNewsIntegrationConfig> options,
            ILogger<MarketNewsIntegration> logger)
        {
            _http = httpClient;
            _config = options.Value;
            _logger = logger;
        }

        // ══════════════════════════════════════════════════════════════════════
        // 1. NOTÍCIAS + SENTIMENTO + AÇÕES IMPACTADAS  →  /v1/news/all
        //    Disponível: FREE | Retorna: notícia, data, sentimento, entidades
        // ══════════════════════════════════════════════════════════════════════
        public Task<MarketNewsAllResponse> GetNewsAsync(
            MarketNewsQueryParams queryParams,
            CancellationToken ct = default)
            => CallAsync<MarketNewsAllResponse>("v1/news/all", q =>
            {
                if (!string.IsNullOrWhiteSpace(queryParams.Symbols)) q["symbols"] = queryParams.Symbols;
                if (!string.IsNullOrWhiteSpace(queryParams.EntityTypes)) q["entity_types"] = queryParams.EntityTypes;
                if (!string.IsNullOrWhiteSpace(queryParams.Industries)) q["industries"] = queryParams.Industries;
                if (!string.IsNullOrWhiteSpace(queryParams.Countries)) q["countries"] = queryParams.Countries;
                if (!string.IsNullOrWhiteSpace(queryParams.Language)) q["language"] = queryParams.Language;
                if (!string.IsNullOrWhiteSpace(queryParams.Search)) q["search"] = queryParams.Search;
                if (!string.IsNullOrWhiteSpace(queryParams.Sort)) q["sort"] = queryParams.Sort;
                if (!string.IsNullOrWhiteSpace(queryParams.SortOrder)) q["sort_order"] = queryParams.SortOrder;
                if (!string.IsNullOrWhiteSpace(queryParams.PublishedOn)) q["published_on"] = queryParams.PublishedOn;
                if (!string.IsNullOrWhiteSpace(queryParams.PublishedAfter)) q["published_after"] = queryParams.PublishedAfter;
                if (!string.IsNullOrWhiteSpace(queryParams.PublishedBefore)) q["published_before"] = queryParams.PublishedBefore;
                if (queryParams.SentimentGte.HasValue) q["sentiment_gte"] = queryParams.SentimentGte.ToString();
                if (queryParams.SentimentLte.HasValue) q["sentiment_lte"] = queryParams.SentimentLte.ToString();
                if (queryParams.FilterEntities.HasValue) q["filter_entities"] = queryParams.FilterEntities.ToString()!.ToLower();
                if (queryParams.MustHaveEntities.HasValue) q["must_have_entities"] = queryParams.MustHaveEntities.ToString()!.ToLower();
                if (queryParams.GroupSimilar.HasValue) q["group_similar"] = queryParams.GroupSimilar.ToString()!.ToLower();
                if (queryParams.Limit.HasValue) q["limit"] = queryParams.Limit.ToString();
                if (queryParams.Page.HasValue) q["page"] = queryParams.Page.ToString();
            }, ct);

        // ══════════════════════════════════════════════════════════════════════
        // 2. NOTÍCIA COMPLETA POR UUID  →  /v1/news/uuid/{uuid}
        //     Use o uuid retornado em GetNewsAsync
        // ══════════════════════════════════════════════════════════════════════
        public Task<NewsArticle> GetNewsByUuidAsync(string uuid, CancellationToken ct = default)
            => CallAsync<NewsArticle>($"v1/news/uuid/{uuid}", _ => { }, ct);

        // ══════════════════════════════════════════════════════════════════════
        // 3. NOTÍCIAS SIMILARES  →  /v1/news/similar/{uuid}
        //     Útil para aprofundar contexto de uma notícia
        // ══════════════════════════════════════════════════════════════════════
        public Task<MarketNewsAllResponse> GetSimilarNewsAsync(
            string uuid,
            string? language = "en",
            string? publishedOn = null,
            CancellationToken ct = default)
            => CallAsync<MarketNewsAllResponse>($"v1/news/similar/{uuid}", q =>
            {
                if (!string.IsNullOrWhiteSpace(language)) q["language"] = language;
                if (!string.IsNullOrWhiteSpace(publishedOn)) q["published_on"] = publishedOn;
            }, ct);

        // ══════════════════════════════════════════════════════════════════════
        // ENGINE INTERNA — HttpClient + timeout + url builder
        // ══════════════════════════════════════════════════════════════════════
        private async Task<T> CallAsync<T>(
            string path,
            Action<System.Collections.Specialized.NameValueCollection> buildQuery,
            CancellationToken ct)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            buildQuery(query);
            query["api_token"] = _config.ApiKey;

            var url = $"{_config.BaseUrl.TrimEnd('/')}/{path}?{query}";
            _logger.LogInformation("MarketNews → GET {Url}", url);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(_config.Timeout));

            try
            {
                using var response = await _http.GetAsync(url, cts.Token);
                var content = await response.Content.ReadAsStringAsync(cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("MarketNews {Status}: {Body}", (int)response.StatusCode, content);
                    throw new HttpRequestException($"MarketNews {(int)response.StatusCode}: {content}");
                }

                return JsonSerializer.Deserialize<T>(content, _jsonOptions)
                    ?? throw new InvalidOperationException("Resposta vazia da API.");
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                _logger.LogError("MarketNews timeout após {Timeout}s", _config.Timeout);
                throw new TimeoutException($"Timeout após {_config.Timeout}s.");
            }
        }
    }
}
