namespace CryptoKeeper.Domain.DataObjects.Dtos.Poloniex
{
    public class TickerDto
    {
        public string FromSymbol { get; set; }
        public string ToSymbol { get; set; }
        public int id { get; set; }
        public string last { get; set; }
        public string lowestAsk { get; set; }
        public string highestBid { get; set; }
        public string percentChange { get; set; }
        public string baseVolume { get; set; }
        public string quoteVolume { get; set; }
        public string isFrozen { get; set; }
        public string high24hr { get; set; }
        public string low24hr { get; set; }
        public bool IsFrozenBool => isFrozen == "1" ? true : false;
    }
}