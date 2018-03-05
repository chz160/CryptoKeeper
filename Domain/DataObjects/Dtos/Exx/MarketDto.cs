namespace CryptoKeeper.Domain.DataObjects.Dtos.Exx
{
    public class MarketDto
    {
        public string fromSymbol { get; set; }
        public string toSymbol { get; set; }
        public string minAmount { get; set; }
        public int amountScale { get; set; }
        public int priceScale { get; set; }
        public int maxLevels { get; set; }
        public bool isOpen { get; set; }
    }
}
