namespace CryptoKeeper.Domain.DataObjects.Dtos.Bitstamp
{
    public class TickerDto
    {
        public string high { get; set; }
        public string last { get; set; }
        public string timestamp { get; set; }
        public string bid { get; set; }
        public string vwap { get; set; }
        public string volume { get; set; }
        public string low { get; set; }
        public string ask { get; set; }
        public string open { get; set; }
    }
}