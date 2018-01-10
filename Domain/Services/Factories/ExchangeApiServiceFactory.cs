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
            if (exchange.Name == ExchangeConstants.Coinbase)
            {
                return new CoinbaseApiService();
            }
            if (exchange.Name == ExchangeConstants.Poloniex)
            {
                return new PoloniexApiService(exchange);
            }
            if (exchange.Name == ExchangeConstants.BitTrex)
            {
                return new BittrexApiService();
            }
            if (exchange.Name == ExchangeConstants.Yobit)
            {
                return new YobitApiService(exchange);
            }
            throw new Exception("Exhcange not found in API factory.");
        }
    }
}
