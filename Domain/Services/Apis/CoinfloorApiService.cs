using System;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class CoinfloorApiService : ApiService
    {
        private readonly Exchange _exchange;

        public CoinfloorApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.Coinfloor;
        public override string PublicUrl => "";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.CryptoCompare;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new CoinfloorPricingMonitorService(this, _exchange);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}