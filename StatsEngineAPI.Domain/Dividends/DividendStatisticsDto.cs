using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Dividends
{
    public record DividendStatisticsDto
    {
        public string Symbol { get; init; } = string.Empty;
        public DividendYieldDto YieldAnualizado { get; init; } = new();
        public DividendGrowthDto CrescimentoDividendos { get; init; } = new();
        public DividendConsistencyDto Consistencia { get; init; } = new();
        public DateTime CalculadoEm { get; init; } = DateTime.UtcNow;
    }
}
