using Microsoft.Extensions.Logging;
using Moq;
using RetailRadar.App.Models;
using RetailRadar.App.Services;

namespace RetailRadar.Tests
{
    public class PriceCheckerServiceTests
    {
        private readonly Mock<ILogger<PriceCheckerService>> _loggerMock;
        private readonly PriceCheckerService _priceCheckerService;

        public PriceCheckerServiceTests()
        {
            _loggerMock = new Mock<ILogger<PriceCheckerService>>();
            _priceCheckerService = new PriceCheckerService(_loggerMock.Object);
        }

        [Fact]
        public async Task ReadPrice_ShouldReturnSuccess_WhenPriceIsFound()
        {
            // Arrange
            var productUrl = "https://www.deporvillage.com/zapatillas-vivobarefoot-primus-lite-knit-azul-marino";
            var expectedPrice = new Price { Amount = 130.00m };

            // Act
            var result = await _priceCheckerService.ExcutePriceDropAlertProcess(productUrl);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedPrice.Amount, result.Value.Amount);
        }
    }
}
