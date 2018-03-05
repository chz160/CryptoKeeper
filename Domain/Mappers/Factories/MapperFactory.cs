using System;
using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers.CryptoCompare;
using CryptoKeeper.Domain.Mappers.Interfaces;
using Newtonsoft.Json.Linq;

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
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.HitBtc.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.WavesDex.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Abucoins.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Binance.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.BitBay.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.BitMarket.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Bitstamp.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Bleutrade.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.BXinth.MarketDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Exmo.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.HuobiPro.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.TrustDex.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Exx.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Gatecoin.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Gemini.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.BitTrex.MarketSummaryDto) && typeof(TOTYPE) == typeof(PricingItem))
            {
                return new PricingItemMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(DataObjects.Dtos.BitTrex.CurrencyDto) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
                typeof(SOURCETYPE) == typeof(KeyValuePair<string, DataObjects.Dtos.Poloniex.CurrencyDto>) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
                typeof(SOURCETYPE) == typeof(DataObjects.Dtos.Bleutrade.CurrencyDto) && typeof(TOTYPE) == typeof(WithdrawalFee))
            {
                return new WithdrawalFeeMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(DataObjects.Dtos.CryptoCompare.SocketDataWrapperDto) && typeof(TOTYPE) == typeof(DataObjects.Dtos.CryptoCompare.TickerDto))
            {
                return new TickerDtoMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(JToken) && typeof(TOTYPE) == typeof(DataObjects.Dtos.TrustDex.TickerDto))
            {
                return new TrustDex.TickerDtoMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(JToken) && typeof(TOTYPE) == typeof(DataObjects.Dtos.Exx.MarketDto))
            {
                return new Exx.MarketDtoMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            if (typeof(SOURCETYPE) == typeof(JToken) && typeof(TOTYPE) == typeof(DataObjects.Dtos.Exx.TickerDto))
            {
                return new Exx.TickerDtoMapper() as IUpdateMapper<SOURCETYPE, TOTYPE>;
            }
            throw new ArgumentException("Couldn't create builder!");
        }
    }
}
