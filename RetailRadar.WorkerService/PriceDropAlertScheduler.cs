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

            _logger.LogInformation("Scheduled daily for {@ScheduledTime}, starting in {@InitialDelay}", scheduledTime.ToString("g"), initialDelay);
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

            _logger.LogInformation("Doing work at: {0}", DateTimeOffset.Now);
            var result = await _retailRadarService.ExcutePriceDropAlertProcess(new PriceDropAlertProcessRequest("https://www.deporvillage.com/zapatillas-vivobarefoot-primus-lite-knit-azul-marino", 200M, "Deporvillage"));

            _logger.LogInformation("Work done {0}", result.IsSuccess ? "succesfully" : "with errors");
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
