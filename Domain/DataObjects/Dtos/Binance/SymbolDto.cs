namespace CryptoKeeper.Domain.DataObjects.Dtos.Binance
{
    public class SymbolDto
    {
        public string symbol { get; set; }
        public string status { get; set; }
        public string baseAsset { get; set; }
        public string quoteAsset { get; set; }
    }
}