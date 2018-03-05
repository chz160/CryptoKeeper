using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.TrustDex;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class TrustDexApiService : ApiService
    {
        private readonly Exchange _exchange;

        public TrustDexApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.TrustDex;
        public override string PublicUrl => "https://trustdex.io/market/api";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;
        
        public override IAmPricingMonitor MonitorPrices()
        {
            return new TrustDexPricingMonitorService(this, _exchange);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<JArray>(PublicUrl, "/return_ticket.php");
            var markets = new BuilderFactory().CreateCollection<JToken, TickerDto>(response).Build();
            markets = markets.Where(m =>
                  m.active &&
                  eligibleSymbols.Contains(m.fromSymbol) &&
                  eligibleSymbols.Contains(m.toSymbol) &&
                  !SymbolConstants.FiatCurrency.Contains(m.fromSymbol) &&
                  !SymbolConstants.FiatCurrency.Contains(m.toSymbol)).ToList();
            foreach (var market in markets)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == market.fromSymbol);
                if (coin == null)
                {
                    coin = new Coin { Symbol = market.fromSymbol };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = market.toSymbol });
            }
        }
    }
}