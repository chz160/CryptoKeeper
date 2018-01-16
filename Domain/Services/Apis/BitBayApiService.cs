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
    public class BitBayApiService : ApiService
    {
        private readonly Exchange _exchange;

        public BitBayApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.BitBay;
        public override string PublicUrl => "https://bitbay.net/API/Public";
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
            return new BitBayPricingMonitorService(this, _exchange);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}