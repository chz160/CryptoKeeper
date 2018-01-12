using System;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Services.Apis;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Factories
{
    public class ExchangeApiServiceFactory : IExchangeApiServiceFactory
    {
        public IAmAnApiService Create(Exchange exchange)
        {
            if (exchange.Name == ExchangeConstants.Abucoins)
            {
                return new AbucoinsApiService();
            }
            if (exchange.Name == ExchangeConstants.Binance)
            {
                return new BinanceApiService();
            }
            if (exchange.Name == ExchangeConstants.BitBay)
            {
                return new BitBayApiService();
            }
            if (exchange.Name == ExchangeConstants.Bitfinex)
            {
                return new BitfinexApiService();
            }
            if (exchange.Name == ExchangeConstants.BitMarket)
            {
                return new BitMarketApiService();
            }
            if (exchange.Name == ExchangeConstants.Bitstamp)
            {
                return new BitstampApiService();
            }
            if (exchange.Name == ExchangeConstants.BitTrex)
            {
                return new BitTrexApiService();
            }
            if (exchange.Name == ExchangeConstants.Bleutrade)
            {
                return new BleutradeApiService();
            }
            if (exchange.Name == ExchangeConstants.BtcMarkets)
            {
                return new BtcMarketsApiService();
            }
            if (exchange.Name == ExchangeConstants.BXinth)
            {
                return new BXinthApiService();
            }
            if (exchange.Name == ExchangeConstants.CexIo)
            {
                return new CexIoApiService();
            }
            if (exchange.Name == ExchangeConstants.Coinbase)
            {
                return new CoinbaseApiService();
            }
            if (exchange.Name == ExchangeConstants.Coinfloor)
            {
                return new CoinfloorApiService();
            }
            if (exchange.Name == ExchangeConstants.Coinroom)
            {
                return new CoinroomApiService();
            }
            if (exchange.Name == ExchangeConstants.Cryptopia)
            {
                return new CryptopiaApiService();
            }
            if (exchange.Name == ExchangeConstants.CryptoX)
            {
                return new CryptoXApiService();
            }
            if (exchange.Name == ExchangeConstants.Exmo)
            {
                return new ExmoApiService();
            }
            if (exchange.Name == ExchangeConstants.Exx)
            {
                return new ExxApiService();
            }
            if (exchange.Name == ExchangeConstants.Gatecoin)
            {
                return new GatecoinApiService();
            }
            if (exchange.Name == ExchangeConstants.Gateio)
            {
                return new GateioApiService();
            }
            if (exchange.Name == ExchangeConstants.Gemini)
            {
                return new GeminiApiService();
            }
            if (exchange.Name == ExchangeConstants.HitBtc)
            {
                return new HitBtcApiService();
            }
            if (exchange.Name == ExchangeConstants.HuobiPro)
            {
                return new HuobiProApiService();
            }
            if (exchange.Name == ExchangeConstants.ItBit)
            {
                return new ItBitApiService();
            }
            if (exchange.Name == ExchangeConstants.Kraken)
            {
                return new KrakenApiService();
            }
            if (exchange.Name == ExchangeConstants.Kucoin)
            {
                return new KucoinApiService();
            }
            if (exchange.Name == ExchangeConstants.LakeBtc)
            {
                return new LakeBtcApiService();
            }
            if (exchange.Name == ExchangeConstants.LiveCoin)
            {
                return new LiveCoinApiService();
            }
            if (exchange.Name == ExchangeConstants.LocalBitcoins)
            {
                return new LocalBitcoinsApiService();
            }
            if (exchange.Name == ExchangeConstants.Lykke)
            {
                return new LykkeApiService();
            }
            if (exchange.Name == ExchangeConstants.Okex)
            {
                return new OkexApiService();
            }
            if (exchange.Name == ExchangeConstants.Poloniex)
            {
                return new PoloniexApiService(exchange);
            }
            if (exchange.Name == ExchangeConstants.QuadrigaCx)
            {
                return new QuadrigaCxApiService();
            }
            if (exchange.Name == ExchangeConstants.Quoine)
            {
                return new QuoineApiService();
            }
            if (exchange.Name == ExchangeConstants.TheRockTrading)
            {
                return new TheRockTradingApiService();
            }
            if (exchange.Name == ExchangeConstants.Tidex)
            {
                return new TidexApiService();
            }
            if (exchange.Name == ExchangeConstants.TrustDex)
            {
                return new TrustDexApiService();
            }
            if (exchange.Name == ExchangeConstants.TuxExchange)
            {
                return new TuxExchangeApiService();
            }
            if (exchange.Name == ExchangeConstants.WavesDex)
            {
                return new WavesDexApiService();
            }
            if (exchange.Name == ExchangeConstants.Yobit)
            {
                return new YobitApiService(exchange);
            }
            throw new Exception("Exhcange not found in API factory.");
        }
    }
}