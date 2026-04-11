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
        public DividendStatisticsDto CalcularEstatisticas(
       string symbol,
       List<DividendEntry> dividendos,
       decimal precoAtual,
       decimal? precoMedioCompra = null);
    }
}
