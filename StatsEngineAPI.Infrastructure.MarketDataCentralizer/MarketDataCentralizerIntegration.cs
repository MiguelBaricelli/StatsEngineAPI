using System.Net;
using System.Text.Json;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using StatsEngineAPI.Domain.Models.Infrastructure.MarketDataCentralizer;
using StatsEngineAPI.Infrastructure.MarketDataCentralizer;

public class MarketDataCentralizerIntegration : IMarketDataCentralizerIntegration
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public MarketDataCentralizerIntegration(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // Retry com backoff exponencial + jitter
        var delays = Backoff.ExponentialBackoff(TimeSpan.FromSeconds(1), retryCount: 3);
        var jitter = new Random();

        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(
                delays.Select(d => d + TimeSpan.FromMilliseconds(jitter.Next(0, 200))),
                onRetry: (outcome, timespan, attempt, context) =>
                {
                    Console.WriteLine($"Retry {attempt} after {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()} - waiting {timespan}");
                });
    }

    public async Task<StockDividendResponse> GetDividendsData(string symbol)
    {
        try
        {
            var uri = $"https://api.marketdatacentralizer.com/v1/dividends?symbol={symbol}";

            var response = await _retryPolicy.ExecuteAsync(ct =>
                _httpClient.GetAsync(uri, ct), CancellationToken.None);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new DividendIntegrationException(
                    $"Erro ao buscar dividendos para {symbol}. Status: {(int)response.StatusCode}",
                    response.StatusCode,
                    body);
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<StockDividendResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
                throw new DividendIntegrationException($"Resposta inválida ou vazia para {symbol}.", response.StatusCode, json);

            return result;
        }
        catch (DividendIntegrationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // erro inesperado
            throw new DividendIntegrationException($"Erro inesperado: {ex.Message}", HttpStatusCode.InternalServerError, null, ex);
        }
    }
}

// Exceção customizada para diferenciar erros da integração
public class DividendIntegrationException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseBody { get; }

    public DividendIntegrationException(string message, HttpStatusCode statusCode, string? responseBody, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
