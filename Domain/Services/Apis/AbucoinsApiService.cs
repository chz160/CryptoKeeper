using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Abucoins;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class AbucoinsApiService : ApiService
    {
        private readonly Exchange _exchange;

        public AbucoinsApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.Abucoins;
        public override string PublicUrl => "https://api.abucoins.com";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;
        public override decimal MakerFee => 0m;
        public override decimal TakerFee => 0.001m;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new AbucoinsPricingMonitorService(this, _exchange);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<List<ProductDto>>(PublicUrl, "/products");
            var products = response.Where(m => eligibleSymbols.Contains(m.base_currency));
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.base_currency);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.base_currency };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.quote_currency });
            }
        }
    }
}