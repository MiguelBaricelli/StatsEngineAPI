using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Infrastructure.Mongo.Configuration
{
    public class MongoConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public Dictionary<string, string> Collections { get; set; }
    }
}
