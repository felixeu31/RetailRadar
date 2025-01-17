using RetailRadar.App.Common;
using RetailRadar.App.Models;

namespace RetailRadar.App.PageScrappers
{
    public interface IRetailWebScrapper
    {
        Task<Result<ProductInfoDto?>> GetProductInfo(string productUrl);
    }
}