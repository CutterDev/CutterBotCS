using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using CutterDragon;

namespace CutterBotCS.JobService
{
    /// <summary>
    /// Worker
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private CutterDragonWorker m_CutterDragon;

        /// <summary>
        /// Constructor
        /// </summary>
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            m_CutterDragon = new CutterDragonWorker();
        }

        /// <summary>
        /// Execute A Sync Worker Job
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                m_CutterDragon.GetAssets();
                await Task.Delay(new TimeSpan(1, 0, 0, 0));
            }
        }
    }
}
