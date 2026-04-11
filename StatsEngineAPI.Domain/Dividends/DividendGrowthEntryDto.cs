using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Dividends
{
    public class DividendGrowthEntryDto
    {
        public string PaymentDate { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal CrescimentoVsAnterior { get; set; }
    }
}
