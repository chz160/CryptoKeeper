using Newtonsoft.Json;

namespace CryptoKeeper.Domain.DataObjects.Dtos.WavesDex
{
    public class TickerDto
    {
        public string Symbol { get; set; }
        public string FromSymbol => Symbol.Split("/")[0];
        public string ToSymbol => Symbol.Split("/")[1];
        public string AmountAssetId { get; set; }
        public string AmountAssetName { get; set; }
        public int AmountAssetDecimals { get; set; }
        public string AmountAssetTotalSupply { get; set; }
        public string AmountAssetMaxSupply { get; set; }
        public string AmountAssetCirculatingSupply { get; set; }
        public string PriceAssetId { get; set; }
        public string PriceAssetName { get; set; }
        public int PriceAssetDecimals { get; set; }
        public string PriceAssetTotalSupply { get; set; }
        public string PriceAssetMaxSupply { get; set; }
        public string PriceAssetCirculatingSupply { get; set; }
        [JsonProperty(PropertyName = "24h_open")]
        public string Open24Hour { get; set; }
        [JsonProperty(PropertyName = "24h_high")]
        public string High24Hour { get; set; }
        [JsonProperty(PropertyName = "24h_low")]
        public string Low24Hour { get; set; }
        [JsonProperty(PropertyName = "24h_close")]
        public string Close24Hour { get; set; }
        [JsonProperty(PropertyName = "24h_vwap")]
        public string Vwrap24Hour { get; set; }
        [JsonProperty(PropertyName = "24h_volume")]
        public string Volume24Hour { get; set; }
        [JsonProperty(PropertyName = "24h_priceVolume")]
        public string PriceVolume24Hour { get; set; }
        public  long Timestamp { get; set; }
    }
}
