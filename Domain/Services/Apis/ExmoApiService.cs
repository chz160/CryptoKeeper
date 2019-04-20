using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Exmo;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class ExmoApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public ExmoApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.Exmo;
        public override string PublicUrl => "https://api.exmo.com/v1";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new ExmoPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            //var symbols = Get<List<string>>(PublicUrl, "/currency").ToList();
            //symbols = symbols.Where(eligibleSymbols.Contains).ToList();
            var tickers = Get<Dictionary<string, TickerDto>>(PublicUrl, "/ticker");
            foreach (var ticker in tickers)
            {
                ticker.Value.from_symbol = ticker.Key.Split("_")[0];
                ticker.Value.to_symbol = ticker.Key.Split("_")[1];
            }
            var products = tickers.Select(m => m.Value).Where(m =>
                  eligibleSymbols.Contains(m.from_symbol) &&
                  eligibleSymbols.Contains(m.to_symbol) &&
                  !SymbolConstants.FiatCurrency.Contains(m.from_symbol) &&
                  !SymbolConstants.FiatCurrency.Contains(m.to_symbol)).ToList();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.from_symbol);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.from_symbol };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.to_symbol });
            }
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}