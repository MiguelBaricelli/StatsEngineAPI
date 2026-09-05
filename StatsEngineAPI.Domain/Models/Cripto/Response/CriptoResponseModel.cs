using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Worker.Models
{
    public class CriptoResponseModel
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime DateInsert { get; set; }
        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }
    }
}
