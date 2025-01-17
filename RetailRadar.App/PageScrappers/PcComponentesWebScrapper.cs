using Microsoft.Playwright;

using RetailRadar.App.Common;
using RetailRadar.App.Models;
using RetailRadar.App.PageScrappers;

namespace RetailRadar.App.PageScrappers;

public class PcComponentesWebScrapper : IRetailWebScrapper
{
    public async Task<Result<ProductInfoDto?>> GetProductInfo(string productUrl)
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(productUrl);

            // Reject cookies
            var rejectButton = await page.QuerySelectorAsync("#cookiesrejectAll");
            if (rejectButton != null)
            {
                await rejectButton.ClickAsync();
            }

            // Extract price
            var priceElement = await page.QuerySelectorAsync("#pdp-price-current-integer");
            if (priceElement == null)
            {
                var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"screenshot{DateTime.Now.Ticks}.png");
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                return Result<ProductInfoDto?>.Failure("Price not found");
            }

            var priceText = await priceElement.InnerTextAsync();
            var priceResult = Price.ConvertFromText(priceText);
            if (!priceResult.IsSuccess)
            {
                return Result<ProductInfoDto?>.Failure(priceResult.ErrorMessage);
            }

            var price = priceResult.Value;

            // Extract title
            var titleElement = await page.QuerySelectorAsync("#pdp-title");
            if (titleElement == null)
            {
                return Result<ProductInfoDto?>.Failure("Title not found");
            }

            var title = (await titleElement.InnerTextAsync()).Trim();

            var productInfo = new ProductInfoDto(title, price, productUrl);

            return Result<ProductInfoDto?>.Success(productInfo);
        }
        catch (Exception ex)
        {
            return Result<ProductInfoDto?>.Failure(ex.Message);
        }
    }
}
