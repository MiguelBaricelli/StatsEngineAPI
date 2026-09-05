using Microsoft.Extensions.Hosting;
using StatsEngineAPI.Worker.Models;
using System.ComponentModel;
using System.Threading;

namespace StatsEngineAPI.Worker
{
    public class StatisticsWorker : BackgroundService
    {

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
              
            };
            }
        }
    }
