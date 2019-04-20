using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BXinth;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class BXinthApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public BXinthApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.BXinth;
        public override string PublicUrl => "https://bx.in.th/api";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new BXinthPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var pairings = Get<Dictionary<string, PairingDto>>(PublicUrl, "/pairing").Select(m=>m.Value).ToList();
            var activePairings = pairings.Where(m => m.active).ToList();
            var products = activePairings.Where(m =>
                eligibleSymbols.Contains(m.primary_currency) &&
                eligibleSymbols.Contains(m.secondary_currency) &&
                !SymbolConstants.FiatCurrency.Contains(m.primary_currency) &&
                !SymbolConstants.FiatCurrency.Contains(m.secondary_currency)).ToList();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.secondary_currency);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.secondary_currency };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.primary_currency });
            }
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}