using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.CexIo;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class CexIoApiService : ApiService
    {
        private readonly Exchange _exchange;

        public CexIoApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.CexIo;
        public override string PublicUrl => "https://cex.io/api";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            throw new NotImplementedException("Pricing monitor needs to be written for CexIo");
            return new NullMonitorService();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var pairings = Get<ResponseDto<MarketDto>>(PublicUrl, "/currency_limits").data.pairs.ToList();
            var products = pairings.Where(m =>
                eligibleSymbols.Contains(m.symbol1) &&
                eligibleSymbols.Contains(m.symbol2) &&
                !SymbolConstants.FiatCurrency.Contains(m.symbol1) &&
                !SymbolConstants.FiatCurrency.Contains(m.symbol2)).ToList();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.symbol1);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.symbol1 };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.symbol2 });
            }
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}