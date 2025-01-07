using RetailRadar.App.Services;

namespace RetailRadar.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPriceCheckerService _retailRadarService;

        public Worker(ILogger<Worker> logger, IPriceCheckerService priceCheckerService)
        {
            _logger = logger;
            this._retailRadarService = priceCheckerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var result = await _retailRadarService.ReadPrice("https://www.deporvillage.com/zapatillas-vivobarefoot-primus-lite-knit-azul-marino");
                    _logger.LogInformation("Result: {0}", result.IsSuccess ? "success" : "failure");
                    _logger.LogInformation("Price: {0}", result.Value!.Amount);
                }
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
