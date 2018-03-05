namespace CryptoKeeper.Domain.DataObjects.Dtos.Gatecoin
{
    public class TickerDto
    {
        public string fromSymbol { get; set; }
        public string toSymbol { get; set; }
        public string currencyPair { get; set; }
        public decimal open { get; set; }
        public decimal last { get; set; }
        public decimal lastQ { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal volume { get; set; }
        public decimal volumn { get; set; }
        public decimal bid { get; set; }
        public decimal bidQ { get; set; }
        public decimal ask { get; set; }
        public decimal askQ { get; set; }
        public decimal vwap { get; set; }
        public long createDateTime { get; set; }
    }
}
