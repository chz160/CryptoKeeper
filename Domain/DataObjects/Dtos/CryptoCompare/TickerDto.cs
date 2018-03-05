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
        public decimal Bid { get; set; }
        public decimal Offer { get; set; }
        public long LastUpdate { get; set; }
        public decimal Avg { get; set; }
        public decimal LastVolume { get; set; }
        public decimal LastVolumeTo { get; set; }
        public decimal LastTradeId { get; set; }
        public decimal VolumeHour { get; set; }
        public decimal VolumeHourTo { get; set; }
        public decimal Volume24Hour { get; set; }
        public decimal Volume24HourTo { get; set; }
        public decimal OpenHour { get; set; }
        public decimal HighHour { get; set; }
        public decimal LowHour { get; set; }
        public decimal Open24Hour { get; set; }
        public decimal High24Hour { get; set; }
        public decimal Low24Hour { get; set; }
        public decimal LastMarket { get; set; }
    }
}