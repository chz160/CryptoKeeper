using System;

namespace CryptoKeeper.Domain.DataObjects.Dtos
{
    public class PricingItem
    {
        public long Timestamp { get; set; }
        public decimal Price { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal Volume { get; set; }

        public DateTimeOffset Date => DateTimeOffset.FromUnixTimeSeconds(Timestamp);
    }
}