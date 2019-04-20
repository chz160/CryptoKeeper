using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.WavesDex;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class WavesDexApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public WavesDexApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.WavesDex;
        public override string PublicUrl => "http://marketdata.wavesplatform.com/api";
        public override string PrivateUrl => "";
        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;
        
        protected override void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body)
        {
            //Do nothing
        }

        public override IAmPricingMonitor MonitorPrices()
        {
            return new WavesDexPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<List<MarketDto>>(PublicUrl, "/markets");
            var markets = response.Where(m => 
                eligibleSymbols.Contains(m.FromSymbol) && 
                eligibleSymbols.Contains(m.ToSymbol) && 
                !SymbolConstants.FiatCurrency.Contains(m.FromSymbol) && 
                !SymbolConstants.FiatCurrency.Contains(m.ToSymbol));
            foreach (var market in markets)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == market.FromSymbol);
                if (coin == null)
                {
                    coin = new Coin { Symbol = market.FromSymbol };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = market.ToSymbol });
            }
        }
    }
}