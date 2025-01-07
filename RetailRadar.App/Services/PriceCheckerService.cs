using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using RetailRadar.App.Common;
using RetailRadar.App.Interfaces.Notifications;
using RetailRadar.App.Models;
using RetailRadar.App.PageScrappers.Deporvillage;

namespace RetailRadar.App.Services
{
    public class PriceCheckerService : IPriceCheckerService
    {
        private readonly ILogger<PriceCheckerService> _logger;
        private readonly IDeporvillageProductPage _deporvillageProductPage;
        private readonly INotificationService _notificationService;

        public PriceCheckerService(ILogger<PriceCheckerService> logger, IDeporvillageProductPage deporvillageProductPage, INotificationService notificationService)
        {
            _logger = logger;
            _deporvillageProductPage = deporvillageProductPage;
            this._notificationService = notificationService;
        }

        public async Task<Result> ExcutePriceDropAlertProcess(string productUrl)
        {
            try
            {
                var productInfoResult = await _deporvillageProductPage.GetProductInfo(productUrl);

                if (productInfoResult == null || !productInfoResult.IsSuccess)
                {
                    return Result.Failure(productInfoResult!.ErrorMessage);
                }

                var productInfo = productInfoResult.Value;

                _logger.LogInformation("Product Info retrieved from web page: {@ProductInfo}", productInfo);

                if (productInfo!.Price.Amount < 200)
                {
                    var notifyTo = new PersonDto("Félix Díez", "felixeu31@gmail.com");
                    var notificationResult = await _notificationService.NotifyProductPriceDrop(productInfo, notifyTo);

                    if (notificationResult == null || !notificationResult.IsSuccess)
                    {
                        return Result.Failure(notificationResult!.ErrorMessage);
                    }

                    _logger.LogInformation("Price drop notified to: {@Person}", notifyTo); 
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
