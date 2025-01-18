namespace RetailRadar.App.PageScrappers
{
    public interface IWebScrapperFactory
    {
        IRetailWebScrapper Create(string WebScrapperType);
    }
}