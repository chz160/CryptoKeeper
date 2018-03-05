using System;
using System.Collections.Generic;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Coinbase;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using Newtonsoft.Json.Linq;

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
            //if (typeof(FROMTYPE) == typeof(HistoMinuteItem) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(SocketDataWrapperDto) && typeof(TOTYPE) == typeof(TickerDto) ||
            //    typeof(FROMTYPE) == typeof(TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(TickerChannelDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.HitBtc.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.WavesDex.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Abucoins.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Binance.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.BitBay.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.BitMarket.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Bitstamp.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Bleutrade.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.BXinth.MarketDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Exmo.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.HuobiPro.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.TrustDex.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.BitTrex.MarketSummaryDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.BitTrex.CurrencyDto) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
            //    typeof(FROMTYPE) == typeof(KeyValuePair<string, DataObjects.Dtos.Poloniex.CurrencyDto>) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Bleutrade.CurrencyDto) && typeof(TOTYPE) == typeof(WithdrawalFee) ||
            //    typeof(FROMTYPE) == typeof(JToken) && typeof(TOTYPE) == typeof(DataObjects.Dtos.TrustDex.TickerDto) ||
            //    typeof(FROMTYPE) == typeof(JToken) && typeof(TOTYPE) == typeof(DataObjects.Dtos.Exx.MarketDto) ||
            //    typeof(FROMTYPE) == typeof(JToken) && typeof(TOTYPE) == typeof(DataObjects.Dtos.Exx.TickerDto) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Exx.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Gatecoin.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(DataObjects.Dtos.Gemini.TickerDto) && typeof(TOTYPE) == typeof(PricingItem) ||
            //    typeof(FROMTYPE) == typeof(ApiConfigurationData) && typeof(TOTYPE) == typeof(Exchange))
            //{
            //    return new CreationBuilder<FROMTYPE, TOTYPE>(fromtype) as IBuilder<TOTYPE>;
            //}
            return new CreationBuilder<FROMTYPE, TOTYPE>(fromtype) as IBuilder<TOTYPE>;
        }

        public IBuilder<IEnumerable<TOTYPE>> CreateCollection<FROMTYPE, TOTYPE>(IEnumerable<FROMTYPE> fromtype) where TOTYPE : class, new()
        {
            return new CollectionBuilder<FROMTYPE, TOTYPE>(fromtype, this);
        }
    }
}
