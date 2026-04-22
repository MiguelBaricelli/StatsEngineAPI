using Microsoft.Extensions.Configuration;
using StatsEngineAPI.Domain.Dividends;
using StatsEngineAPI.Domain.Models.NewsService;
using System.Net.Http;
using System.Text.Json;

namespace StatsEngineAPI.Infrastructure.Repository.Nasdaq
{
    public class AlphaDividendsConsumer
    {
        private readonly HttpClient _httpClient;
        public AlphaDividendsConsumer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<StockDividendModelResponse> AlphaDividensConsumer(string symbol)
        {
            return symbol switch
            {
                "IBM" => Task.FromResult(GetIbmMock()),
                "PETR4" => Task.FromResult(GetPetr4Mock()),
                "VALE3" => Task.FromResult(GetVale3Mock()),
                "BBSE3" => Task.FromResult(GetBbse3Mock()),
                "MXRF11" => Task.FromResult(GetMxrf11Mock()),
                _ => Task.FromResult(new StockDividendModelResponse
                {
                    Symbol = symbol,
                    Data = new List<DividendEntry>()
                })
            };
        }

        private static StockDividendModelResponse GetIbmMock() => new()
        {
            Symbol = "IBM",
            Price = "26",
            Data = new List<DividendEntry>
    {
        new() { ExDividendDate = "2026-02-10", DeclarationDate = "2026-01-28", RecordDate = "2026-02-10", PaymentDate = "2026-03-10", Amount = "1.68"},
        new() { ExDividendDate = "2025-11-10", DeclarationDate = "2025-10-22", RecordDate = "2025-11-10", PaymentDate = "2025-12-10", Amount = "1.68"},
        new() { ExDividendDate = "2025-08-08", DeclarationDate = "2025-07-23", RecordDate = "2025-08-08", PaymentDate = "2025-09-10", Amount = "1.68"},
        new() { ExDividendDate = "2025-05-09", DeclarationDate = "2025-04-29", RecordDate = "2025-05-09", PaymentDate = "2025-06-10", Amount = "1.68"},
        new() { ExDividendDate = "2025-02-10", DeclarationDate = "2025-01-28", RecordDate = "2025-02-10", PaymentDate = "2025-03-10", Amount = "1.67"},
        new() { ExDividendDate = "2024-11-12", DeclarationDate = "2024-10-30", RecordDate = "2024-11-12", PaymentDate = "2024-12-10", Amount = "1.67"},
        new() { ExDividendDate = "2024-08-09", DeclarationDate = "2024-07-29", RecordDate = "2024-08-09", PaymentDate = "2024-09-10", Amount = "1.67"},
        new() { ExDividendDate = "2024-05-09", DeclarationDate = "2024-04-30", RecordDate = "2024-05-10", PaymentDate = "2024-06-10", Amount = "1.67"},
        new() { ExDividendDate = "2024-02-08", DeclarationDate = "2024-01-30", RecordDate = "2024-02-09", PaymentDate = "2024-03-09", Amount = "1.66"},
        new() { ExDividendDate = "2023-11-09", DeclarationDate = "2023-10-30", RecordDate = "2023-11-10", PaymentDate = "2023-12-09", Amount = "1.66"},
        new() { ExDividendDate = "2023-08-09", DeclarationDate = "2023-07-24", RecordDate = "2023-08-10", PaymentDate = "2023-09-09", Amount = "1.66"},
        new() { ExDividendDate = "2023-05-09", DeclarationDate = "2023-04-25", RecordDate = "2023-05-10", PaymentDate = "2023-06-10", Amount = "1.66"},
        new() { ExDividendDate = "2023-02-09", DeclarationDate = "2023-01-31", RecordDate = "2023-02-10", PaymentDate = "2023-03-10", Amount = "1.65"},
        new() { ExDividendDate = "2022-11-09", DeclarationDate = "2022-10-25", RecordDate = "2022-11-10", PaymentDate = "2022-12-10", Amount = "1.65"},
        new() { ExDividendDate = "2022-08-09", DeclarationDate = "2022-07-25", RecordDate = "2022-08-10", PaymentDate = "2022-09-10", Amount = "1.65"},
        new() { ExDividendDate = "2022-05-09", DeclarationDate = "2022-04-26", RecordDate = "2022-05-10", PaymentDate = "2022-06-10", Amount = "1.65"},
        new() { ExDividendDate = "2022-02-10", DeclarationDate = "2022-02-01", RecordDate = "2022-02-11", PaymentDate = "2022-03-10", Amount = "1.64"},
        new() { ExDividendDate = "2021-11-09", DeclarationDate = "2021-10-26", RecordDate = "2021-11-10", PaymentDate = "2021-12-10", Amount = "1.64"},
        new() { ExDividendDate = "2021-08-09", DeclarationDate = "2021-07-27", RecordDate = "2021-08-10", PaymentDate = "2021-09-10", Amount = "1.64"},
        new() { ExDividendDate = "2021-05-07", DeclarationDate = "2021-04-27", RecordDate = "2021-05-10", PaymentDate = "2021-06-10", Amount = "1.64"},
        new() { ExDividendDate = "2021-02-09", DeclarationDate = "2021-01-26", RecordDate = "2021-02-10", PaymentDate = "2021-03-10", Amount = "1.63"}
    }
        };

        private static StockDividendModelResponse GetPetr4Mock() => new()
        {
            Symbol = "PETR4",
            Data = new List<DividendEntry>
            {
                new() { ExDividendDate = "2026-04-22", DeclarationDate = "2026-03-05", RecordDate = "2026-04-22", PaymentDate = "2026-06-22", Amount = "0.3131" },
                new() { ExDividendDate = "2025-12-22", DeclarationDate = "2025-11-06", RecordDate = "2025-12-22", PaymentDate = "2026-03-20", Amount = "0.2964" },
                new() { ExDividendDate = "2025-09-20", DeclarationDate = "2025-08-30", RecordDate = "2025-09-20", PaymentDate = "2025-10-20", Amount = "0.45" },
                new() { ExDividendDate = "2025-06-02", DeclarationDate = "2025-05-12", RecordDate = "2025-06-02", PaymentDate = "2025-08-20", Amount = "0.4546" },
                new() { ExDividendDate = "2025-03-15", DeclarationDate = "2025-02-28", RecordDate = "2025-03-15", PaymentDate = "2025-04-15", Amount = "0.32" },
                new() { ExDividendDate = "2024-12-20", DeclarationDate = "2024-11-30", RecordDate = "2024-12-20", PaymentDate = "2025-03-20", Amount = "0.29" },
                new() { ExDividendDate = "2024-09-20", DeclarationDate = "2024-08-30", RecordDate = "2024-09-20", PaymentDate = "2024-10-20", Amount = "0.45" },
                new() { ExDividendDate = "2024-06-02", DeclarationDate = "2024-05-12", RecordDate = "2024-06-02", PaymentDate = "2024-08-20", Amount = "0.44" },
                new() { ExDividendDate = "2024-03-15", DeclarationDate = "2024-02-28", RecordDate = "2024-03-15", PaymentDate = "2024-04-15", Amount = "0.31" },
                new() { ExDividendDate = "2023-12-20", DeclarationDate = "2023-11-30", RecordDate = "2023-12-20", PaymentDate = "2024-03-20", Amount = "0.28" },
                new() { ExDividendDate = "2023-09-20", DeclarationDate = "2023-08-30", RecordDate = "2023-09-20", PaymentDate = "2023-10-20", Amount = "0.42" },
                new() { ExDividendDate = "2023-06-02", DeclarationDate = "2023-05-12", RecordDate = "2023-06-02", PaymentDate = "2023-08-20", Amount = "0.41" }
            }
        };

        private static StockDividendModelResponse GetVale3Mock() => new()
        {
            Symbol = "VALE3",
            Data = new List<DividendEntry>
            {
                new() { ExDividendDate = "2026-03-15", DeclarationDate = "2026-02-28", RecordDate = "2026-03-15", PaymentDate = "2026-03-25", Amount = "2.10" },
                new() { ExDividendDate = "2025-09-20", DeclarationDate = "2025-08-30", RecordDate = "2025-09-20", PaymentDate = "2025-09-30", Amount = "1.53" },
                new() { ExDividendDate = "2025-03-15", DeclarationDate = "2025-02-28", RecordDate = "2025-03-15", PaymentDate = "2025-03-25", Amount = "2.20" },
                new() { ExDividendDate = "2024-09-29", DeclarationDate = "2024-09-10", RecordDate = "2024-09-29", PaymentDate = "2024-10-10", Amount = "1.54" },
                new() { ExDividendDate = "2024-03-15", DeclarationDate = "2024-02-28", RecordDate = "2024-03-15", PaymentDate = "2024-03-25", Amount = "2.00" },
                new() { ExDividendDate = "2023-09-20", DeclarationDate = "2023-08-30", RecordDate = "2023-09-20", PaymentDate = "2023-09-30", Amount = "1.50" },
                new() { ExDividendDate = "2023-03-15", DeclarationDate = "2023-02-28", RecordDate = "2023-03-15", PaymentDate = "2023-03-25", Amount = "2.10" }
            }
        };

        private static StockDividendModelResponse GetBbse3Mock() => new()
        {
            Symbol = "BBSE3",
            Data = new List<DividendEntry>
            {
                new() { ExDividendDate = "2026-03-20", DeclarationDate = "2026-03-01", RecordDate = "2026-03-20", PaymentDate = "2026-03-30", Amount = "0.49" },
                new() { ExDividendDate = "2025-09-20", DeclarationDate = "2025-09-01", RecordDate = "2025-09-20", PaymentDate = "2025-09-30", Amount = "0.47" },
                new() { ExDividendDate = "2025-03-20", DeclarationDate = "2025-03-01", RecordDate = "2025-03-20", PaymentDate = "2025-03-30", Amount = "0.46" },
                new() { ExDividendDate = "2024-09-20", DeclarationDate = "2024-09-01", RecordDate = "2024-09-20", PaymentDate = "2024-09-30", Amount = "0.45" },
                new() { ExDividendDate = "2024-03-20", DeclarationDate = "2024-03-01", RecordDate = "2024-03-20", PaymentDate = "2024-03-30", Amount = "0.44" },
                new() { ExDividendDate = "2023-09-20", DeclarationDate = "2023-09-01", RecordDate = "2023-09-20", PaymentDate = "2023-09-30", Amount = "0.43" },
                new() { ExDividendDate = "2023-03-20", DeclarationDate = "2023-03-01", RecordDate = "2023-03-20", PaymentDate = "2023-03-30", Amount = "0.42" }
            }
        };
        private static StockDividendModelResponse GetMxrf11Mock() => new()
        {
            Symbol = "MXRF11",
            Data = new List<DividendEntry>
    {
        new() { ExDividendDate = "2026-03-10", DeclarationDate = "2026-03-01", RecordDate = "2026-03-10", PaymentDate = "2026-03-15", Amount = "0.11" },
        new() { ExDividendDate = "2026-02-10", DeclarationDate = "2026-02-01", RecordDate = "2026-02-10", PaymentDate = "2026-02-15", Amount = "0.11" },
        new() { ExDividendDate = "2026-01-10", DeclarationDate = "2026-01-01", RecordDate = "2026-01-10", PaymentDate = "2026-01-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-12-10", DeclarationDate = "2025-12-01", RecordDate = "2025-12-10", PaymentDate = "2025-12-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-11-10", DeclarationDate = "2025-11-01", RecordDate = "2025-11-10", PaymentDate = "2025-11-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-10-10", DeclarationDate = "2025-10-01", RecordDate = "2025-10-10", PaymentDate = "2025-10-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-09-10", DeclarationDate = "2025-09-01", RecordDate = "2025-09-10", PaymentDate = "2025-09-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-08-10", DeclarationDate = "2025-08-01", RecordDate = "2025-08-10", PaymentDate = "2025-08-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-07-10", DeclarationDate = "2025-07-01", RecordDate = "2025-07-10", PaymentDate = "2025-07-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-06-10", DeclarationDate = "2025-06-01", RecordDate = "2025-06-10", PaymentDate = "2025-06-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-05-10", DeclarationDate = "2025-05-01", RecordDate = "2025-05-10", PaymentDate = "2025-05-15", Amount = "0.11" },
        new() { ExDividendDate = "2025-04-10", DeclarationDate = "2025-04-01", RecordDate = "2025-04-10", PaymentDate = "2025-04-15", Amount = "0.11" }
    }
        };
    }
}

