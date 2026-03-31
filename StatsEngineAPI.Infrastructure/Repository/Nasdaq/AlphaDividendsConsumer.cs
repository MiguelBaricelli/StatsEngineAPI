using StatsEngineAPI.Domain.Dividends;

namespace StatsEngineAPI.Infrastructure.Repository.Nasdaq
{
    public class AlphaDividendsConsumer
    {
        public AlphaDividendsConsumer()
        {

        }

        public Task<StockDividendModelResponse> AlphaDividensConsumer()
        {
            StockDividendModelResponse mockData = new StockDividendModelResponse()
            {
                Symbol = "IBM",
                Data = new List<DividendEntry>()
        {
            new DividendEntry() { ExDividendDate = "2026-02-10", DeclarationDate = "2026-01-28", RecordDate = "2026-02-10", PaymentDate = "2026-03-10", Amount = "1.68" },
            new DividendEntry() { ExDividendDate = "2025-11-10", DeclarationDate = "2025-10-22", RecordDate = "2025-11-10", PaymentDate = "2025-12-10", Amount = "1.68" },
            new DividendEntry() { ExDividendDate = "2025-08-08", DeclarationDate = "2025-07-23", RecordDate = "2025-08-08", PaymentDate = "2025-09-10", Amount = "1.68" },
            new DividendEntry() { ExDividendDate = "2025-05-09", DeclarationDate = "2025-04-29", RecordDate = "2025-05-09", PaymentDate = "2025-06-10", Amount = "1.68" },
            new DividendEntry() { ExDividendDate = "2025-02-10", DeclarationDate = "2025-01-28", RecordDate = "2025-02-10", PaymentDate = "2025-03-10", Amount = "1.67" },
            new DividendEntry() { ExDividendDate = "2024-11-12", DeclarationDate = "2024-10-30", RecordDate = "2024-11-12", PaymentDate = "2024-12-10", Amount = "1.67" },
            new DividendEntry() { ExDividendDate = "2024-08-09", DeclarationDate = "2024-07-29", RecordDate = "2024-08-09", PaymentDate = "2024-09-10", Amount = "1.67" },
            new DividendEntry() { ExDividendDate = "2024-05-09", DeclarationDate = "2024-04-30", RecordDate = "2024-05-10", PaymentDate = "2024-06-10", Amount = "1.67" },
            new DividendEntry() { ExDividendDate = "2024-02-08", DeclarationDate = "2024-01-30", RecordDate = "2024-02-09", PaymentDate = "2024-03-09", Amount = "1.66" },
            new DividendEntry() { ExDividendDate = "2023-11-09", DeclarationDate = "2023-10-30", RecordDate = "2023-11-10", PaymentDate = "2023-12-09", Amount = "1.66" },
            new DividendEntry() { ExDividendDate = "2023-08-09", DeclarationDate = "2023-07-24", RecordDate = "2023-08-10", PaymentDate = "2023-09-09", Amount = "1.66" },
            new DividendEntry() { ExDividendDate = "2023-05-09", DeclarationDate = "2023-04-25", RecordDate = "2023-05-10", PaymentDate = "2023-06-10", Amount = "1.66" },
            new DividendEntry() { ExDividendDate = "2023-02-09", DeclarationDate = "2023-01-31", RecordDate = "2023-02-10", PaymentDate = "2023-03-10", Amount = "1.65" },
            new DividendEntry() { ExDividendDate = "2022-11-09", DeclarationDate = "2022-10-25", RecordDate = "2022-11-10", PaymentDate = "2022-12-10", Amount = "1.65" },
            new DividendEntry() { ExDividendDate = "2022-08-09", DeclarationDate = "2022-07-25", RecordDate = "2022-08-10", PaymentDate = "2022-09-10", Amount = "1.65" },
            new DividendEntry() { ExDividendDate = "2022-05-09", DeclarationDate = "2022-04-26", RecordDate = "2022-05-10", PaymentDate = "2022-06-10", Amount = "1.65" },
            new DividendEntry() { ExDividendDate = "2022-02-10", DeclarationDate = "2022-02-01", RecordDate = "2022-02-11", PaymentDate = "2022-03-10", Amount = "1.64" },
            new DividendEntry() { ExDividendDate = "2021-11-09", DeclarationDate = "2021-10-26", RecordDate = "2021-11-10", PaymentDate = "2021-12-10", Amount = "1.64" },
            new DividendEntry() { ExDividendDate = "2021-08-09", DeclarationDate = "2021-07-27", RecordDate = "2021-08-10", PaymentDate = "2021-09-10", Amount = "1.64" },
            new DividendEntry() { ExDividendDate = "2021-05-07", DeclarationDate = "2021-04-27", RecordDate = "2021-05-10", PaymentDate = "2021-06-10", Amount = "1.64" },
            new DividendEntry() { ExDividendDate = "2021-02-09", DeclarationDate = "2021-01-26", RecordDate = "2021-02-10", PaymentDate = "2021-03-10", Amount = "1.63" }
        }
            };

            return Task.FromResult(mockData);
        }

    }
}
