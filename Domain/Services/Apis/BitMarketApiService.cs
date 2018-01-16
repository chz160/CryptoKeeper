using System.Net;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class BitMarketApiService : ApiService
    {
        private readonly Exchange _exchange;

        public BitMarketApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.BitMarket;
        public override string PublicUrl => "https://www.bitmarket.pl/json";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        protected override void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body)
        { }

        public override IAmPricingMonitor MonitorPrices()
        {
            return new BitMarketPricingMonitorService(this, _exchange);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}