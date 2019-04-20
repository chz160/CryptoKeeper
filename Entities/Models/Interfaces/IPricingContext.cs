using Microsoft.EntityFrameworkCore;

namespace CryptoKeeper.Entities.Pricing.Models.Interfaces
{
    public interface IPricingContext
    {
        DbSet<OrderBook> OrderBooks { get; set; }
        DbSet<WithdrawalFee> WithdrawalFees { get; set; }

        int SaveChanges();
    }
}