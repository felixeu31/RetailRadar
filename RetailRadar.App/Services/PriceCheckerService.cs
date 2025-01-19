using Microsoft.Extensions.Logging;

using RetailRadar.App.Common;
using RetailRadar.App.Interfaces.Notifications;
using RetailRadar.App.Models;
using RetailRadar.App.PageScrappers;

namespace RetailRadar.App.Services
{
    public class PriceCheckerService : IPriceCheckerService
    {
        private readonly ILogger<PriceCheckerService> _logger;
        private readonly INotificationService _notificationService;
        private readonly IWebScrapperFactory _webScraperFactory;

        public PriceCheckerService(ILogger<PriceCheckerService> logger, INotificationService notificationService, IWebScrapperFactory webScrapperFactory)
        {
            _logger = logger;
            this._notificationService = notificationService;
            this._webScraperFactory = webScrapperFactory;
        }

        public async Task<Result> ExcutePriceDropAlertProcess(PriceDropAlertProcessRequest request)
        {
            try
            {
                var webScrapper = _webScraperFactory.Create(request.WebScrapperType);
                var productInfoResult = await webScrapper.GetProductInfo(request.ProductUrl);

                if (productInfoResult == null || !productInfoResult.IsSuccess)
                {
                    return Result.Failure(productInfoResult!.ErrorMessage);
                }

                var productInfo = productInfoResult.Value;

                _logger.LogInformation("Product Info retrieved from web page: {@ProductInfo}", productInfo);

                if (productInfo!.Price.Amount < request.PriceThreshold)
                {
                    var notificationResult = await _notificationService.NotifyProductPriceDrop(productInfo, request.Recipient);

                    if (notificationResult == null || !notificationResult.IsSuccess)
                    {
                        return Result.Failure(notificationResult!.ErrorMessage);
                    }

                    _logger.LogInformation("Price drop notified to: {@Person}", request.Recipient); 
                }
                else
                {
                    _logger.LogInformation("Price is not below {@PriceThreshold}, no notification sent", request.PriceThreshold);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }

    public record PriceDropAlertProcessRequest(string ProductUrl, decimal PriceThreshold, string WebScrapperType, PersonDto Recipient);
}
