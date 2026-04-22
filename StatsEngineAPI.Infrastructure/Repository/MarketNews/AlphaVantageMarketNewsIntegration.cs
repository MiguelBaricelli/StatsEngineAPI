using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using StatsEngineAPI.Domain.Interfaces.Infra.MarketNews;
using StatsEngineAPI.Domain.Models;
using StatsEngineAPI.Domain.Models.AlphaVantage;
using StatsEngineAPI.Domain.Models.MarketNews;
using StatsEngineAPI.Infrastructure.Repository.MarketNews.Config;
using System.Globalization;
using System.Net;
using System.Text.Json;

public class AlphaVantageMarketNewsIntegration : IAlphaVantageMarketNewsIntegration
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AlphaVantageMarketNewsIntegration> _logger;
    private readonly AlphaMarketNewsConfig _config;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public AlphaVantageMarketNewsIntegration(
        HttpClient httpClient,
        ILogger<AlphaVantageMarketNewsIntegration> logger,
        IOptions<AlphaMarketNewsConfig> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = options.Value ?? throw new ArgumentNullException(nameof(options)); ;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        _retryPolicy = BuildRetryPolicy();
    }

    private AsyncRetryPolicy<HttpResponseMessage> BuildRetryPolicy()
    {
        var jitterer = new Random();
        var delays = Backoff.ExponentialBackoff(TimeSpan.FromSeconds(_config.InitialRetryDelaySeconds), retryCount: _config.RetryCount);

        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(msg => (int)msg.StatusCode >= 500 || msg.StatusCode == HttpStatusCode.RequestTimeout || msg.StatusCode == (HttpStatusCode)429)
            .WaitAndRetryAsync(
                delays.Select(d => TimeSpan.FromMilliseconds(d.TotalMilliseconds + jitterer.Next(0, 200))).ToArray(),
                onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
                {
                    if (outcome.Exception != null)
                    {
                        _logger.LogWarning(outcome.Exception, "Retry {RetryAttempt} after exception when calling news endpoint. Waiting {Delay}.", retryAttempt, timespan);
                    }
                    else
                    {
                        var status = (int)outcome.Result.StatusCode;
                        _logger.LogWarning("Retry {RetryAttempt} after HTTP {StatusCode} when calling news endpoint. Waiting {Delay}.", retryAttempt, status, timespan);

                        if (outcome.Result.StatusCode == (HttpStatusCode)429)
                        {
                            if (outcome.Result.Headers.TryGetValues("Retry-After", out var values))
                            {
                                var retryAfter = values.FirstOrDefault();
                                _logger.LogWarning("Server returned 429 with Retry-After: {RetryAfter}", retryAfter);
                            }
                        }
                    }

                    await Task.CompletedTask;
                });
    }

    public async Task<NewsFeedResponse?> GetLatestNewsAsync(string? symbol = null, CancellationToken cancellationToken = default)
    {
        var requestUri = _config.BaseUrl;
        var endpoint = _config.NewsEndpoint;
        string requestUriFinal;

        if (symbol != null)
        {
            requestUriFinal = $"{requestUri}{endpoint}?function=NEWS_SENTIMENT&tickers={symbol}&apikey={_config.ApiKey}";

        }
        else
        {
            requestUriFinal = $"{requestUri}{endpoint}?function=NEWS_SENTIMENT&apikey={_config.ApiKey}";

        }


        _logger.LogInformation("Requesting market news from {Endpoint}", requestUri);

        try
        {
            HttpResponseMessage response = await _retryPolicy.ExecuteAsync(async ct =>
            {
                var req = new HttpRequestMessage(HttpMethod.Get, requestUriFinal);

                // Se ApiKey estiver configurada na config e o provedor aceitar header, adicione aqui
                if (!string.IsNullOrWhiteSpace(_config.ApiKey))
                {
                    if (!req.Headers.Contains("x-api-key"))
                        req.Headers.Add("x-api-key", _config.ApiKey);
                }

                _logger.LogInformation("Sending HTTP GET to {Uri}", requestUri);
                var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

                // Se 429 e Retry-After presente, respeitar antes de retornar
                if (resp.StatusCode == (HttpStatusCode)429)
                {
                    if (resp.Headers.TryGetValues("Retry-After", out var values))
                    {
                        var retryAfterValue = values.FirstOrDefault();
                        if (int.TryParse(retryAfterValue, out var seconds))
                        {
                            _logger.LogWarning("Received 429. Respecting Retry-After: {Seconds}s", seconds);
                            await Task.Delay(TimeSpan.FromSeconds(seconds), ct);
                        }
                        else if (DateTimeOffset.TryParse(retryAfterValue, out var date))
                        {
                            var wait = date - DateTimeOffset.UtcNow;
                            if (wait > TimeSpan.Zero)
                            {
                                _logger.LogWarning("Received 429. Respecting Retry-After until {Date}. Waiting {Wait}.", date, wait);
                                await Task.Delay(wait, ct);
                            }
                        }
                    }
                }

                return resp;
            }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("News endpoint returned {StatusCode}. Reading content.", (int)response.StatusCode);
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var result = await JsonSerializer.DeserializeAsync<NewsFeedResponse>(stream, _jsonOptions, cancellationToken);
                _logger.LogInformation("Successfully deserialized news feed. Items: {Count}", result.Items == null);
                return result;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to fetch news after retries. StatusCode: {StatusCode}. Body: {Body}", (int)response.StatusCode, body);
            return null;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("GetLatestNewsAsync was cancelled by caller.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching market news.");
            throw new Exception(ex.Message);
        }
    }

    // ================================
    // ALPHA VANTAGE — NYSE / NASDAQ
    // ================================

    public async Task<AssetQuoteUniq?> GetAlphaVantageQuoteAsync(string symbol, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_config.ApiKey}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("AlphaVantage RAW response: {Json}", json);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<AlphaVantageQuoteResponse>(json, options);

            _logger.LogInformation("GlobalQuote deserialized: {Quote}",
            JsonSerializer.Serialize(data?.GlobalQuote));

            var q = data?.GlobalQuote;
            if (q == null || string.IsNullOrEmpty(q.Price)) return null;

            decimal.TryParse(q.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var price);
            decimal.TryParse(q.Change, NumberStyles.Any, CultureInfo.InvariantCulture, out var change);
            decimal.TryParse(q.High, NumberStyles.Any, CultureInfo.InvariantCulture, out var high);
            decimal.TryParse(q.Low, NumberStyles.Any, CultureInfo.InvariantCulture, out var low);
            decimal.TryParse(q.Volume, NumberStyles.Any, CultureInfo.InvariantCulture, out var volume);

            var changePercentStr = q.ChangePercent.Replace("%", "").Trim();
            decimal.TryParse(changePercentStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var changePercent);

            return new AssetQuoteUniq
            {
                Symbol = q.Symbol,
                Name = q.Symbol,
                Market = "NYSE/NASDAQ",
                Price = price,
                Change = change,
                ChangePercent = changePercent,
                DayHigh = high,
                DayLow = low,
                Volume = volume,
                Currency = "USD"
            };
        }
        catch
        {
            return null;
        }
    }
}
