using Microsoft.Extensions.Logging;
using Moq;

using RetailRadar.App.Common;
using RetailRadar.App.Models;
using RetailRadar.App.PageScrappers;
using RetailRadar.App.Services;

namespace RetailRadar.Tests
{
    public class PcComponentesWebScrapperTests
    {
        private readonly PcComponentesWebScrapper _pcComponentesProductPage;

        public PcComponentesWebScrapperTests()
        {
            _pcComponentesProductPage = new PcComponentesWebScrapper();
        }

        [Fact]
        public async Task GetProductInfo_ShouldReturnSuccess()
        {
            // Arrange
            var productUrl = "https://www.pccomponentes.com/portatil-lenovo-ideapad-slim-3-15irh8-intel-core-i7-13620h-16gb-1tb-ssd-156";

            // Act
            var result = await _pcComponentesProductPage.GetProductInfo(productUrl);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(!string.IsNullOrEmpty(result.Value.Name));
            Assert.True(result.Value.Price.Amount > 0);
        }
    }
}
