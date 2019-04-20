using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.HitBtc;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class HitBtcApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPricingService _pricingService;
        private readonly IBuilderFactory _builderFactory;

        public HitBtcApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.HitBtc;
        public override string PublicUrl => "https://api.hitbtc.com/api/2/public";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.WebSocket;
        public override decimal MakerFee => 0.001m;
        public override decimal TakerFee => 0.001m;
        
        protected override void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body)
        { }

        public override IAmPricingMonitor MonitorPrices()
        {
            return new HitBtcPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<List<CurrencyDto>>(PublicUrl, "/currency");
            var products = response.Where(m => eligibleSymbols.Contains(m.Id) && m.Crypto && m.PayinEnabled && m.PayoutEnabled && m.TransferEnabled);
            var markets = GetSymbols();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.Id);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.Id };
                    foreach (var market in markets.Where(m => m.BaseCurrency == product.Id && eligibleSymbols.Contains(m.QuoteCurrency)))
                    {
                        coin.Coins.Add(new Coin { Symbol = market.QuoteCurrency });
                    }

                    if (coin.Coins.Any())
                    {
                        exchange.Coins.Add(coin);
                    }
                }
                //coin.Coins.Add(new Coin { Symbol = product.Key == SymbolConstants.Btc ? SymbolConstants.Usd : SymbolConstants.Btc });
            }
        }

        private List<SymbolDto> GetSymbols()
        {
            return Get<List<SymbolDto>>(PublicUrl, "/symbol");
        }
    }
}