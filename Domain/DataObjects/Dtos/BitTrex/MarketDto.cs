namespace CryptoKeeper.Domain.DataObjects.Dtos.BitTrex
{
    public class MarketDto
    {
        public string MarketCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public string MarketCurrencyLong { get; set; }
        public string BaseCurrencyLong { get; set; }
        public decimal MinTradeSize { get; set; }
        public string MarketName { get; set; }
        public bool IsActive { get; set; }
        public string Created { get; set; }
        public string Notice { get; set; }
        public bool? IsSponsored { get; set; }
        public string LogoUrl { get; set; }
    }
}
