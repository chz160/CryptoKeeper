using System;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Services.Apis;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Factories
{
    public class ExchangeApiServiceFactory : IExchangeApiServiceFactory
    {
        private readonly IApiServiceInjectionFactory _apiServiceInjectionFactory;

        public ExchangeApiServiceFactory(IApiServiceInjectionFactory apiServiceInjectionFactory)
        {
            _apiServiceInjectionFactory = apiServiceInjectionFactory;
        }
        public IAmAnApiService Create(Exchange exchange)
        {
            //if (exchange.Name == ExchangeConstants.Abucoins)
            //{
            //    return _apiServiceInjectionFactory.Create<AbucoinsApiService>(exchange);
            //}
            if (exchange.Name == ExchangeConstants.Binance)
            {
                return _apiServiceInjectionFactory.Create<BinanceApiService>(exchange);
            }
            //if (exchange.Name == ExchangeConstants.BitBay)
            //{
            //    return _apiServiceInjectionFactory.Create<BitBayApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.Bitfinex)
            //{
            //    return _apiServiceInjectionFactory.Create<BitfinexApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.BitMarket)
            //{
            //    return _apiServiceInjectionFactory.Create<BitMarketApiService>(exchange);
            //}
            if (exchange.Name == ExchangeConstants.Bitstamp)
            {
                return _apiServiceInjectionFactory.Create<BitstampApiService>(exchange);
            }
            if (exchange.Name == ExchangeConstants.BitTrex)
            {
                return _apiServiceInjectionFactory.Create<BitTrexApiService>();
            }
            //if (exchange.Name == ExchangeConstants.Bleutrade)
            //{
            //    return _apiServiceInjectionFactory.Create<BleutradeApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.BtcMarkets)
            //{
            //    return _apiServiceInjectionFactory.Create<BtcMarketsApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.BXinth)
            //{
            //    return _apiServiceInjectionFactory.Create<BXinthApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.CexIo)
            //{
            //    return _apiServiceInjectionFactory.Create<CexIoApiService>(exchange);
            //}
            if (exchange.Name == ExchangeConstants.Coinbase)
            {
                return _apiServiceInjectionFactory.Create<CoinbaseApiService>();
            }
            //if (exchange.Name == ExchangeConstants.Coinfloor)
            //{
            //    return _apiServiceInjectionFactory.Create<CoinfloorApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.Coinroom)
            //{
            //    return _apiServiceInjectionFactory.Create<CoinroomApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Cryptopia)
            //{
            //    return _apiServiceInjectionFactory.Create<CryptopiaApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.CryptoX)
            //{
            //    return _apiServiceInjectionFactory.Create<CryptoXApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Exmo)
            //{
            //    return _apiServiceInjectionFactory.Create<ExmoApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.Exx)
            //{
            //    return _apiServiceInjectionFactory.Create<ExxApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.Gatecoin)
            //{
            //    return _apiServiceInjectionFactory.Create<GatecoinApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.Gateio)
            //{
            //    return _apiServiceInjectionFactory.Create<GateioApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Gemini)
            //{
            //    return _apiServiceInjectionFactory.Create<GeminiApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.HitBtc)
            //{
            //    return _apiServiceInjectionFactory.Create<HitBtcApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.HuobiPro)
            //{
            //    return _apiServiceInjectionFactory.Create<HuobiProApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.ItBit)
            //{
            //    return _apiServiceInjectionFactory.Create<ItBitApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Kraken)
            //{
            //    return _apiServiceInjectionFactory.Create<KrakenApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Kucoin)
            //{
            //    return _apiServiceInjectionFactory.Create<KucoinApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.LakeBtc)
            //{
            //    return _apiServiceInjectionFactory.Create<LakeBtcApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.LiveCoin)
            //{
            //    return _apiServiceInjectionFactory.Create<LiveCoinApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.LocalBitcoins)
            //{
            //    return _apiServiceInjectionFactory.Create<LocalBitcoinsApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Lykke)
            //{
            //    return _apiServiceInjectionFactory.Create<LykkeApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Okex)
            //{
            //    return _apiServiceInjectionFactory.Create<OkexApiService>();
            //}
            if (exchange.Name == ExchangeConstants.Poloniex)
            {
                return _apiServiceInjectionFactory.Create<PoloniexApiService>(exchange);
            }
            //if (exchange.Name == ExchangeConstants.QuadrigaCx)
            //{
            //    return _apiServiceInjectionFactory.Create<QuadrigaCxApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Quoine)
            //{
            //    return _apiServiceInjectionFactory.Create<QuoineApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.TheRockTrading)
            //{
            //    return _apiServiceInjectionFactory.Create<TheRockTradingApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.Tidex)
            //{
            //    return _apiServiceInjectionFactory.Create<TidexApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.TrustDex)
            //{
            //    return _apiServiceInjectionFactory.Create<TrustDexApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.TuxExchange)
            //{
            //    return _apiServiceInjectionFactory.Create<TuxExchangeApiService>();
            //}
            //if (exchange.Name == ExchangeConstants.WavesDex)
            //{
            //    return _apiServiceInjectionFactory.Create<WavesDexApiService>(exchange);
            //}
            //if (exchange.Name == ExchangeConstants.Yobit)
            //{
            //    return _apiServiceInjectionFactory.Create<YobitApiService>(exchange);
            //}
            throw new Exception("Exchange not found in API factory.");
        }
    }
}