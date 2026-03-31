using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Helpers
{
    public class ParseDecimalHelper
    {
        public decimal ParseDecimal(string? value)
       => decimal.TryParse(value, System.Globalization.NumberStyles.Any,
           System.Globalization.CultureInfo.InvariantCulture, out var result)
           ? result : 0;
    }
}
