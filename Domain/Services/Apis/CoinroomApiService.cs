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
    public class CoinroomApiService : ApiService
    {
        public CoinroomApiService(IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider)
            : base(configService, cryptoCompareDataService, serviceProvider)
        { }

        public override string Name => ExchangeConstants.Coinroom;
        public override string PublicUrl => "https://coinroom.com/api";
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

        // DOESN'T SEEM TO HAVE A WAY TO GET ALT PRICES IN BTC
        //public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        //{
        //    var pairs = Get<List<TraidingPairInfoDto>>(PublicUrl, "/availableCurrencies");
        //    var activePairs = pairs.Where(m => m.trading == "Enabled").ToList();
        //    var products = activePairs.Where(m => eligibleSymbols.Contains(m.FromSymbol) && eligibleSymbols.Contains(m.ToSymbol)).ToList();
        //    foreach (var product in products)
        //    {
        //        var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.FromSymbol);
        //        if (coin == null)
        //        {
        //            coin = new Coin { Symbol = product.FromSymbol };
        //            exchange.Coins.Add(coin);
        //        }
        //        coin.Coins.Add(new Coin { Symbol = product.ToSymbol });
        //    }
        //}

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}