using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Dividends
{
    public record DividendYieldDto
    {
        public decimal TotalPagoNoPeriodo { get; init; }
        public decimal PrecoAtual { get; init; }
        public decimal YieldPercentual { get; init; }
        public string Classificacao { get; init; } = string.Empty; // "Baixo", "Moderado", "Alto"
    }
}
