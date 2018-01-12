using System;
using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers.CryptoCompare;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers.Factories
{
    public class MapperFactory : IMapperFactory
    {
        public IUpdateMapper<SOURCETYPE, TOTYPE> CreateUpdate<SOURCETYPE, TOTYPE>() where TOTYPE : class, new()
        {
            if (typeof(SOURCETYPE) == typeof(ApiConfigurationData) && typeof(TOTYPE) == typeof(Exchange))
            {
                return new ExchangeMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(object) && typeof(TOTYPE) == typeof(Coin))
            {
                return new CoinMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(object) && typeof(TOTYPE) == typeof(CoinExchange))
            {
                return new CoinExchangeMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(DataObjects.Dtos.CryptoCompare.HistoMinuteItem) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Coinbase.TickerChannelDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.CryptoCompare.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.BitTrex.MarketSummaryDto) && typeof(TOTYPE) == typeof(PricingItem))
            {
                return new PricingItemMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(DataObjects.Dtos.BitTrex.CurrencyDto) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
                typeof(SOURCETYPE) == typeof(KeyValuePair<string, DataObjects.Dtos.Poloniex.CurrencyDto>) && typeof(TOTYPE) == typeof(WithdrawalFee))
            {
                return new WithdrawalFeeMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(string[]) && typeof(TOTYPE) == typeof(DataObjects.Dtos.CryptoCompare.TickerDto))
            {
                return new TickerDtoMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            throw new ArgumentException("Couldn't create builder!");
        }
    }
}
