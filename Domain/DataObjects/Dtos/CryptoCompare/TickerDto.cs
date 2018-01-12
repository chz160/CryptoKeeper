namespace CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare
{
    public class TickerDto
    {
        public string MessageType { get; set; }
        public string Market { get; set; }
        public string FromSymbol { get; set; }
        public string ToSymbol { get; set; }
        public string Flags { get; set; }
        public decimal Price { get; set; }
        public string Bid { get; set; }
        public string Offer { get; set; }
        public decimal LastUpdate { get; set; }
        public string Avg { get; set; }
        public decimal LastVolume { get; set; }
        public decimal LastVolumeTo { get; set; }
        public decimal LastTradeId { get; set; }
        public string VolumeHour { get; set; }
        public string VolumeHourTo { get; set; }
        public decimal Volume24Hour { get; set; }
        public decimal Volume24HourTo { get; set; }
        public string OpenHour { get; set; }
        public string HighHour { get; set; }
        public string LowHour { get; set; }
        public string Open24Hour { get; set; }
        public string High24Hour { get; set; }
        public string Low24Hour { get; set; }
        public string LastMarket { get; set; }
        public long Timestamp { get; set; }
    }
}