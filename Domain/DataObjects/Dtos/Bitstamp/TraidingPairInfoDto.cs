namespace CryptoKeeper.Domain.DataObjects.Dtos.Bitstamp
{
    public class TraidingPairInfoDto
    {
        public string name { get; set; }
        public int base_decimals { get; set; }
        public string minimum_order { get; set; }
        public int counter_decimals { get; set; }
        public string trading { get; set; }
        public string url_symbol { get; set; }
        public string description { get; set; }

        public string FromSymbol => name.Split("/")[0];
        public string ToSymbol => name.Split("/")[1];
    }
}
