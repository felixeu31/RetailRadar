using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using RetailRadar.App.Common;
using RetailRadar.App.PageScrappers.Deporvillage;

namespace RetailRadar.App.Services
{
    public class PriceCheckerService : IPriceCheckerService
    {
        private readonly ILogger<PriceCheckerService> _logger;
        private readonly IDeporvillageProductPage _deporvillageProductPage;

        public PriceCheckerService(ILogger<PriceCheckerService> logger, IDeporvillageProductPage deporvillageProductPage)
        {
            _logger = logger;
            _deporvillageProductPage = deporvillageProductPage;
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

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
