using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Application.Helpers
{
    public class ParseDataHelper
    {
        public  DateTime ParseData(string data)
       => DateTime.TryParse(data, out var result) ? result : DateTime.MinValue;
    }
}
