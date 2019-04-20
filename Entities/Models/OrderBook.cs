using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoKeeper.Entities.Pricing.Models
{
    public class OrderBook
    {
        public OrderBook()
        {
            CreatedDate = DateTime.Now;
        }

        public int Id { get; set; }
        public string Key { get; set; }
        public long Timestamp { get; set; }
        public decimal Price { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal Volume { get; set; }
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public DateTimeOffset Date => DateTimeOffset.FromUnixTimeSeconds(Timestamp);
    }
}
