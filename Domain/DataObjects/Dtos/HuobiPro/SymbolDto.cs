using Newtonsoft.Json;

namespace CryptoKeeper.Domain.DataObjects.Dtos.HuobiPro
{
    public class SymbolDto
    {
        [JsonProperty(PropertyName = "base-currency")]
        public string basecurrency { get; set; }
        [JsonProperty(PropertyName = "quote-currency")]
        public string quotecurrency { get; set; }
        [JsonProperty(PropertyName = "price-precision")]
        public int priceprecision { get; set; }
        [JsonProperty(PropertyName = "amount-precision")]
        public int amountprecision { get; set; }
        [JsonProperty(PropertyName = "symbol-partition")]
        public string symbolpartition { get; set; }

    }
}