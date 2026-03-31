using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Dividends
{
    public record DividendGrowthEntryDto
    {
        public string PaymentDate { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public decimal CrescimentoVsAnterior { get; init; }
    }
}
