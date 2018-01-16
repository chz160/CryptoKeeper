namespace CryptoKeeper.Domain.DataObjects.Dtos.BitMarket
{
    public class TickerDto
    {
        public decimal ask { get; set; }
        public decimal bid { get; set; }
        public decimal last { get; set; }
        public decimal low { get; set; }
        public decimal high { get; set; }
        public decimal vwap { get; set; }
        public decimal volume { get; set; }
    }
}
