namespace RetailRadar.App.PageScrappers;

public class WebScrapperFactory : IWebScrapperFactory
{
    public IRetailWebScrapper Create(string WebScrapperType)
    {
        return WebScrapperType switch
        {
            "PcComponentes" => new PcComponentesWebScrapper(),
            "Deporvillage" => new DeporvillageWebScrapper(),
            _ => throw new ArgumentException("Invalid WebScrapperType"),
        };
    }
}
