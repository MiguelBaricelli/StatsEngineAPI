using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Models.NewsService
{
    /// <summary>
    /// Sinal de trade gerado a partir do sentimento do mercado.
    /// </summary>
    public class TradeSignalResult
    {
        public string Signal { get; set; } = string.Empty;
        public decimal Sentiment { get; set; }
        public string Reasoning { get; set; } = string.Empty;
    }
}
