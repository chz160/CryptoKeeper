using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.HuobiPro;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class HuobiProApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public HuobiProApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.HuobiPro;
        public override string PublicUrl => "https://api.huobi.pro";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new HuobiProPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var symbols = Get<ResponseDto<List<SymbolDto>>>(PublicUrl, "/v1/common/symbols").data.Where(m=> 
                eligibleSymbols.Contains(m.basecurrency.ToUpper()) &&
                eligibleSymbols.Contains(m.quotecurrency.ToUpper())).ToList();
            foreach (var symbol in symbols)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == symbol.basecurrency.ToUpper());
                if (coin == null)
                {
                    coin = new Coin { Symbol = symbol.basecurrency.ToUpper() };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = symbol.quotecurrency.ToUpper() });
            }
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}