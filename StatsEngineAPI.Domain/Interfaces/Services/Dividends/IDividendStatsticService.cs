using StatsEngineAPI.Domain.Dividends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Interfaces.Services.Dividends
{
    public interface IDividendStatsticService
    {
        /// <summary>
        /// Calcula todas as estatísticas de dividendos de uma vez.
        /// </summary>
        DividendStatisticsDto CalcularEstatisticas(string symbol, List<DividendEntry> dividendos, decimal precoAtual);

        /// <summary>
        /// Consistência de pagamento: frequência e regularidade dos dividendos.
        /// </summary>
        DividendConsistencyDto CalcularConsistencia(List<DividendEntry> dividendos, int diasGapAlerta = 90);
    }
}
