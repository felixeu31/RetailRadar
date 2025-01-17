using Microsoft.Extensions.Logging;

using Moq;

using RetailRadar.App.Common;
using RetailRadar.App.Models;
using RetailRadar.App.PageScrappers;
using RetailRadar.App.Services;

namespace RetailRadar.Tests
{
    public class DeporvillageWebScrapperTests
    {
        private readonly DeporvillageWebScrapper _deporvillageProductPage;

        public DeporvillageWebScrapperTests()
        {
            _deporvillageProductPage = new DeporvillageWebScrapper();
        }

        [Fact]
        public async Task GetProductInfo_ShouldReturnSuccess()
        {
            // Arrange
            var productUrl = "https://www.deporvillage.com/zapatillas-vivobarefoot-primus-lite-knit-azul-marino";

            // Act
            var result = await _deporvillageProductPage.GetProductInfo(productUrl);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(!string.IsNullOrEmpty(result.Value.Name));
            Assert.True(result.Value.Price.Amount > 0);
        }
    }
}
