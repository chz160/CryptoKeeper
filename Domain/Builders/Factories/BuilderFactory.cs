using System;
using System.Collections.Generic;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Coinbase;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;

namespace CryptoKeeper.Domain.Builders.Factories
{
    public class BuilderFactory : IBuilderFactory
    {
        public IBuilder<TOTYPE> Create<FROMTYPE, TOTYPE>(FROMTYPE fromtype) where TOTYPE : class, new()
        {
            if (typeof(FROMTYPE) == typeof(object) &&
                typeof(TOTYPE) == typeof(Coin))
            {
                return new CoinBuilder(fromtype as Coin) as IBuilder<TOTYPE>;
            }
            if (typeof(FROMTYPE) == typeof(HistoMinuteItem) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(FROMTYPE) == typeof(string[]) && typeof(TOTYPE) == typeof(TickerDto) ||
                typeof(FROMTYPE) == typeof(TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(FROMTYPE) == typeof(TickerChannelDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(FROMTYPE) == typeof(DataObjects.Dtos.BitTrex.MarketSummaryDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(FROMTYPE) == typeof(DataObjects.Dtos.BitTrex.CurrencyDto) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
                typeof(FROMTYPE) == typeof(KeyValuePair<string, DataObjects.Dtos.Poloniex.CurrencyDto>) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
                typeof(FROMTYPE) == typeof(ApiConfigurationData) && typeof(TOTYPE) == typeof(Exchange))
            {
                return new CreationBuilder<FROMTYPE, TOTYPE>(fromtype) as IBuilder<TOTYPE>;
            }
            throw new ArgumentException($"Couldn't create builder for {typeof(FROMTYPE)} and {typeof(TOTYPE)}!");
        }

        public IBuilder<IEnumerable<TOTYPE>> CreateCollection<FROMTYPE, TOTYPE>(IEnumerable<FROMTYPE> fromtype) where TOTYPE : class, new()
        {
            return new CollectionBuilder<FROMTYPE, TOTYPE>(fromtype, this);
        }
    }
}
