using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class TheRockTradingApiService : ApiService
    {
        public TheRockTradingApiService(IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider)
            : base(configService, cryptoCompareDataService, serviceProvider)
        { }

        public override string Name => ExchangeConstants.TheRockTrading;
        public override string PublicUrl => "";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.CryptoCompare;

        protected override void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body)
        {
            throw new System.NotImplementedException();
        }

        public override IAmPricingMonitor MonitorPrices()
        {
            return new NullMonitorService();
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}