using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Exx;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class ExxApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public ExxApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.Exx;
        public override string PublicUrl => "https://api.exx.com/data/v1";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new ExxPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<JObject>(PublicUrl, "/markets");
            var markets = new BuilderFactory().CreateCollection<JToken, MarketDto>(response).Build();
            markets = markets.Where(m =>
                m.isOpen &&
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