using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoKeeper.Entities.Pricing.Models.Mappings
{
    public class OrderBookMap : IEntityTypeConfiguration<OrderBook>
    {
        public void Configure(EntityTypeBuilder<OrderBook> builder)
        {
            builder.ToTable("OrderBook");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Ask).HasColumnType("decimal(18, 0)");
            builder.Property(e => e.Bid).HasColumnType("decimal(18, 0)");
            builder.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            builder.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            builder.Property(e => e.Volume).HasColumnType("decimal(18, 0)");
        }
    }
}
