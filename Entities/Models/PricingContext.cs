using System.IO;
using CryptoKeeper.Entities.Pricing.Models.Interfaces;
using CryptoKeeper.Entities.Pricing.Models.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CryptoKeeper.Entities.Pricing.Models
{
    public partial class PricingContext : DbContext, IPricingContext
    {
        public PricingContext()
        { }

        public PricingContext(DbContextOptions<PricingContext> options)
            : base(options)
        { }

        public virtual DbSet<OrderBook> OrderBooks { get; set; }
        public virtual DbSet<WithdrawalFee> WithdrawalFees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
                var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("PricingContext");
                optionsBuilder.UseSqlServer(connectionString);
                base.OnConfiguring(optionsBuilder);
            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new OrderBookMap());
            modelBuilder.ApplyConfiguration(new WithdrawalFeeMap());
        }
    }
}
