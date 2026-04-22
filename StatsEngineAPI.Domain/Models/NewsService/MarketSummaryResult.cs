using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Models.NewsService
{
    /// <summary>
    /// Resumo inteligente das notícias mais relevantes do mercado.
    /// </summary>
    public class MarketSummaryResult
    {
        public string Summary { get; set; } = string.Empty;
        public int Total { get; set; }
        public List<string> Titles { get; set; } = new();
    }
}
