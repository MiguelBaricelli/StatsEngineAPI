using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Dividends
{
    public record DividendConsistencyDto
    {
        public int TotalPagamentos { get; init; }
        public int DiasNoPeriodo { get; init; }
        public double MediaDiasEntrePagamentos { get; init; }
        public decimal ConsistenciaPercentual { get; init; }
        public string Classificacao { get; init; } = string.Empty; // "Irregular", "Regular", "Muito Regular"
        public string? AlertaGap { get; init; } // null se ok, mensagem se houver gap longo
    }
}
