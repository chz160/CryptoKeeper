namespace CryptoKeeper.Domain.DataObjects.Dtos.BitBay
{
    public class TickerDto
    {
        public decimal max { get; set; }
        public decimal min { get; set; }
        public decimal last { get; set; }
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public decimal vwap { get; set; }
        public decimal average { get; set; }
        public decimal volume { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }
}
