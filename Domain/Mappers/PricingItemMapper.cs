using System;
using System.Globalization;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BitTrex;
using CryptoKeeper.Domain.DataObjects.Dtos.Coinbase;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers
{
    public class PricingItemMapper : 
        IUpdateMapper<HistoMinuteItem, PricingItem>,
        IUpdateMapper<TickerChannelDto, PricingItem>,
        IUpdateMapper<MarketSummaryDto, PricingItem>
    {
        public void Update(HistoMinuteItem sourceType, PricingItem updateType)
        {
            updateType.Timestamp = sourceType.time;
            updateType.Price = sourceType.close ?? 0m;
        }

        public void Update(TickerChannelDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Price = decimal.Parse(sourceType.Price, NumberStyles.Float);
        }

        public void Update(MarketSummaryDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.Parse(sourceType.TimeStamp).ToUnixTimeSeconds();
            updateType.Price = sourceType.Bid;
        }
    }
}