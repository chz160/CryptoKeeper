using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoKeeper.Entities.Pricing.Models.Mappings
{
    public class WithdrawalFeeMap : IEntityTypeConfiguration<WithdrawalFee>
    {
        public void Configure(EntityTypeBuilder<WithdrawalFee> builder)
        {
            builder.ToTable("WithdrawalFee");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            builder.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            builder.Property(e => e.Fee).HasColumnType("decimal(18, 0)");
        }
    }
}
