using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Bitstamp;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class BitstampApiService : ApiService
    {
        private readonly Exchange _exchange;

        public BitstampApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.Bitstamp;
        public override string PublicUrl => "https://www.bitstamp.net/api/v2";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new BitstampPricingMonitorService(this, _exchange);
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var pairs = Get<List<TraidingPairInfoDto>>(PublicUrl, "/trading-pairs-info");
            var activePairs = pairs.Where(m => m.trading == "Enabled").ToList();
            var products = activePairs.Where(m => eligibleSymbols.Contains(m.FromSymbol) && eligibleSymbols.Contains(m.ToSymbol)).ToList();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.FromSymbol);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.FromSymbol };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.ToSymbol });
            }
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}