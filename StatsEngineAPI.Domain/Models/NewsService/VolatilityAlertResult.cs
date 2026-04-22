using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Models.NewsService
{
    /// <summary>
    /// Alerta de volatilidade baseado em notícias de alto impacto.
    /// </summary>
    public class VolatilityAlertResult
    {
        public string Volatility { get; set; } = string.Empty;
        public int HighImpactNews { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
