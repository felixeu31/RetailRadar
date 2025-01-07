using Microsoft.Extensions.Logging;
using Moq;

using RetailRadar.App.Common;
using RetailRadar.App.Models;
using RetailRadar.App.PageScrappers.Deporvillage;
using RetailRadar.App.Services;

namespace RetailRadar.Tests
{
    public class DeporvillageProductPageTests
    {
        private readonly DeporvillageProductPage _deporvillageProductPage;

        public DeporvillageProductPageTests()
        {
            _deporvillageProductPage = new DeporvillageProductPage();
        }

        [Fact]
        public async Task GetProductInfo_ShouldReturnSuccess()
        {
            // Arrange
            var productUrl = "https://www.deporvillage.com/zapatillas-vivobarefoot-primus-lite-knit-azul-marino";
            var expectedName = "Zapatillas VivoBarefoot Primus Lite Knit azul marino";
            var expectedPrice = new Price { Amount = 130.00m };

            // Act
            var result = await _deporvillageProductPage.GetProductInfo(productUrl);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedName, result.Value.Name);
            Assert.Equal(expectedPrice.Amount, result.Value.Price.Amount);
        }
    }
}
