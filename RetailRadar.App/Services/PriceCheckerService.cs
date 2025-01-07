using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using RetailRadar.App.Common;

namespace RetailRadar.App.Services
{
    public class PriceCheckerService : IPriceCheckerService
    {
        private readonly ILogger<PriceCheckerService> _logger;

        public PriceCheckerService(ILogger<PriceCheckerService> logger)
        {
            _logger = logger;
        }
        public async Task<Result<Price?>> ExcutePriceDropAlertProcess(string productUrl)
        {
            try
            {
                // Instalar Playwright browsers si es necesario
                var exitCode = Program.Main(new[] { "install", "chromium" });
                if (exitCode != 0)
                {
                    throw new Exception("Error instalando navegadores de Playwright");
                }

                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
                var page = await browser.NewPageAsync();

                await page.GotoAsync(productUrl);

                // Reject cookies
                var rejectButton = await page.QuerySelectorAsync("#onetrust-reject-all-handler");
                if (rejectButton != null)
                {
                    await rejectButton.ClickAsync();
                }

                // Extract price
                var priceElement = await page.QuerySelectorAsync("div[data-testid='price-indication-price']");
                if (priceElement == null)
                {
                    var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"screenshot{DateTime.Now.Ticks}.png");
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                    return Result<Price?>.Failure("Price not found");
                }

                var priceText = (await priceElement.InnerTextAsync()).Trim().Replace("€", "");
                if (decimal.TryParse(priceText, out var priceAmount))
                {
                    var price = new Price { Amount = priceAmount };
                    return Result<Price?>.Success(price);
                }

                return Result<Price?>.Failure("Invalid price format");
            }
            catch (Exception ex)
            {
                return Result<Price?>.Failure(ex.Message);
            }
        }
    }
}
