namespace CryptoKeeper.Domain.DataObjects.Dtos.Bleutrade
{
    public class MarketDto
    {
        public string MarketCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public string MarketCurrencyLong { get; set; }
        public string BaseCurrencyLong { get; set; }
        public string MinTradeSize { get; set; }
        public string MarketName { get; set; }
        public string IsActive { get; set; }
    }
}