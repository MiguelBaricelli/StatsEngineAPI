using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Dividends
{
    public record DividendGrowthDto
    {
        public decimal UltimoDividendo { get; init; }
        public decimal PrimeiroDividendo { get; init; }
        public decimal CrescimentoPercentual { get; init; }
        public string Tendencia { get; init; } = string.Empty; // "Crescendo", "Estável", "Caindo"
        public List<DividendGrowthEntryDto> HistoricoCrescimento { get; init; } = new();
    }
}
