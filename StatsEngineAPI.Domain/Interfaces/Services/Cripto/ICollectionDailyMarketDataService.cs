using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsEngineAPI.Domain.Interfaces.Services.Cripto
{
    public interface ICollectionDailyMarketDataService
    {
        Task CollectDailyDataAsync(CancellationToken cancellationToken);
    }
}
