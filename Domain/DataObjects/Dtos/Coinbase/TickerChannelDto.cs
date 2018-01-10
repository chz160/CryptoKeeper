namespace CryptoKeeper.Domain.DataObjects.Dtos.Coinbase
{
    public class TickerChannelDto : ChannelDto
    {
        public long Sequence { get; set; }
        public string Price { get; set; }
        public string Best_Bid { get; set; }
        public string Best_Ask { get; set; }
        public string Open_24h { get; set; }
        public string Volume_24h { get; set; }
        public string Low_24h { get; set; }
        public string High_24h { get; set; }
        public string Volume_30d { get; set; }
    }
}