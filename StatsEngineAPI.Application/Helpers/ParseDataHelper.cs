using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Helpers
{
    public class ParseDataHelper
    {
        public DateTime ParseData(string data)
       => DateTime.TryParse(data, out var result) ? result : DateTime.MinValue;

        public DateTime ParseDateByAlphaVantage(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return DateTime.MinValue;

            try
            {
                return DateTime.ParseExact(
                    date,
                    "yyyyMMdd'T'HHmmss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
                );
            }
            catch
            {
                return DateTime.MinValue;
            }
        }


    }
}
