
using RetailRadar.App.Common;
using RetailRadar.App.Models;

namespace RetailRadar.App.PageScrappers.Deporvillage
{
    public interface IDeporvillageProductPage
    {
        Task<Result<ProductInfoDto?>> GetProductInfo(string productUrl);
    }
}