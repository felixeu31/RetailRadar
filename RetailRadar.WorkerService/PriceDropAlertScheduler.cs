using RetailRadar.App.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace RetailRadar.WorkerService
{
    public class PriceDropAlertScheduler : BackgroundService
    {
        private readonly ILogger<PriceDropAlertScheduler> _logger;
        private readonly IPriceCheckerService _retailRadarService;
        private Timer _timer;

        public PriceDropAlertScheduler(ILogger<PriceDropAlertScheduler> logger, IPriceCheckerService priceCheckerService)
        {
            _logger = logger;
            _retailRadarService = priceCheckerService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //ScheduleDailyTask(stoppingToken);
            ScheduleMinutlyTask(stoppingToken);
            return Task.CompletedTask;
        }

        private void ScheduleDailyTask(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            var scheduledTime = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0, 0, DateTimeKind.Local);

            if (now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            var initialDelay = scheduledTime - now;
            _timer = new Timer(async state => await DoWork(stoppingToken), null, initialDelay, TimeSpan.FromDays(1));
        }

        private void ScheduleMinutlyTask(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            _timer = new Timer(async state => await DoWork(stoppingToken), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            _logger.LogInformation("Worker '{0}' running at: {1}", nameof(PriceDropAlertScheduler), DateTimeOffset.Now);

            var result = await _retailRadarService.ExcutePriceDropAlertProcess("https://www.deporvillage.com/zapatillas-vivobarefoot-primus-lite-knit-azul-marino");

            _logger.LogInformation("Worker '{0}' finished {1}", nameof(PriceDropAlertScheduler), result.IsSuccess ? "succesfully" : "with errors");
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
