using System;

namespace CryptoKeeper.Entities.Pricing.Models
{
    public class WithdrawalFee
    {
        public WithdrawalFee()
        {
            CreatedDate = DateTime.Now;
        }
        public int Id { get; set; }
        public string Key { get; set; }
        public string Symbol { get; set; }
        public decimal Fee { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
