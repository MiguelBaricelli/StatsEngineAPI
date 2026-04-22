using Microsoft.Extensions.Configuration;
using StatsEngineAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StatsEngineAPI.Infrastructure.Repository.B3
{
    public class BrApiIntegration
    {
        private readonly HttpClient _httpClient;
        private readonly string _brApiToken;

        public BrApiIntegration(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _brApiToken = configuration["BrApi:Token"] ?? string.Empty;

        }

        // Sufixos e padrões que indicam ativo B3
        private static readonly string[] B3Exchanges = { ".SA", ".F" };
        private static readonly int[] B3TickerLengths = { 5, 6 }; // PETR4, VALE3, MGLU3

        public async Task<AssetQuoteUniq?> GetBrApiQuoteAsync(string symbol, CancellationToken cancellationToken)
        {
            try
            {
                // BrApi aceita símbolo sem .SA
                var cleanSymbol = symbol.Replace(".SA", "").Replace(".F", "");
                var tokenParam = string.IsNullOrEmpty(_brApiToken) ? "" : $"&token={_brApiToken}";
                var url = $"https://brapi.dev/api/quote/{cleanSymbol}?fundamental=false{tokenParam}";

                var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<BrApiQuoteResponse>(json, options);

                var result = data?.Results?.FirstOrDefault();
                if (result == null) return null;

                return new AssetQuoteUniq
                {
                    Symbol = result.Symbol,
                    Name = result.LongName.Length > 0 ? result.LongName : result.ShortName,
                    Market = "B3",
                    Price = result.RegularMarketPrice,
                    Change = result.RegularMarketChange,
                    ChangePercent = result.RegularMarketChangePercent,
                    DayHigh = result.RegularMarketDayHigh,
                    DayLow = result.RegularMarketDayLow,
                    Volume = result.RegularMarketVolume,
                    Currency = "BRL"
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
