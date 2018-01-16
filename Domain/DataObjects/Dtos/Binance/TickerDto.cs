using Newtonsoft.Json;

namespace CryptoKeeper.Domain.DataObjects.Dtos.Binance
{
    public class TickerDto
    {
        [JsonProperty(PropertyName = "e")]
        public string EventType { get; set; }
        [JsonProperty(PropertyName = "E")]
        public long EventTime { get; set; }
        [JsonProperty(PropertyName = "s")]
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "p")]
        public string PriceChange { get; set; }
        [JsonProperty(PropertyName = "P")]
        public string PriceChangePercent { get; set; }
        [JsonProperty(PropertyName = "w")]
        public string WeightedAveragePercent { get; set; }
        [JsonProperty(PropertyName = "x")]
        public string PreviousDaysClosePrice { get; set; }
        [JsonProperty(PropertyName = "c")]
        public string CurrentDaysClosePrice { get; set; }
        [JsonProperty(PropertyName = "Q")]
        public string CloseTradesQuantity { get; set; }
        [JsonProperty(PropertyName = "b")]
        public string BestBidPrice { get; set; }
        [JsonProperty(PropertyName = "B")]
        public string BestBidQuantity { get; set; }
        [JsonProperty(PropertyName = "a")]
        public string BestAskPrice { get; set; }
        [JsonProperty(PropertyName = "A")]
        public string BestAskQuantity { get; set; }
        [JsonProperty(PropertyName = "o")]
        public string OpenPrice { get; set; }
        [JsonProperty(PropertyName = "h")]
        public string HighPrice { get; set; }
        [JsonProperty(PropertyName = "l")]
        public string LowPrice { get; set; }
        [JsonProperty(PropertyName = "v")]
        public string TotalTradedBaseAssetVolume { get; set; }
        [JsonProperty(PropertyName = "q")]
        public string TotalTradedQuoteAssetVolume { get; set; }
        [JsonProperty(PropertyName = "O")]
        public long StatisticsOpenTime { get; set; }
        [JsonProperty(PropertyName = "C")]
        public long StatisticsCloseTime { get; set; }
        [JsonProperty(PropertyName = "F")]
        public long FirstTradeId { get; set; }
        [JsonProperty(PropertyName = "L")]
        public long LastTradeId { get; set; }
        [JsonProperty(PropertyName = "n")]
        public long TotalNumberOfTrades { get; set; }
    }
}