using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Models.NewsService
{
    public class ProcessedNews
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public string Region { get; set; }
        public decimal Score { get; set; }
        public string Sentiment { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
