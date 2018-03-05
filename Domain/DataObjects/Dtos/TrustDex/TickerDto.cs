namespace CryptoKeeper.Domain.DataObjects.Dtos.TrustDex
{
    public class TickerDto
    {
        public string id { get; set; }
        public string coin { get; set; }
        public string last { get; set; }
        public string highestBid { get; set; }
        public string lowestAsk { get; set; }
        public string percentChange { get; set; }
        public string baseVolume { get; set; }
        public string quoteVolume { get; set; }
        public string isFrozen { get; set; }
        public string high24hr { get; set; }
        public string low24hr { get; set; }

        public bool active => isFrozen == "0";
        public string fromSymbol => coin.Split("_")[1];
        public string toSymbol => coin.Split("_")[0];
    }
}
