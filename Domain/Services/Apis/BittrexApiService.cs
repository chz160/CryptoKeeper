using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BitTrex;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    //Docs https://bittrex.com/home/api
    public class BitTrexApiService : ApiService
    {
        public override string Name => ExchangeConstants.BitTrex;
        public override string PublicUrl => "https://bittrex.com/api/v1.1";
        public override string PrivateUrl => PublicUrl;
        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;
        public override Boolean PlaceParametersInUrl => true;
        public override HMAC GetHMac()
        {
            return new HMACSHA512(Encoder.GetBytes(Secret));
        }

        public override bool SignAsHex => true;

        protected override void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body)
        {
            //request.Headers.Add("apikey", Key);
            request.Headers.Add("apisign", SignString(baseUrl + relativeUrl));
        }

        public override IAmPricingMonitor MonitorPrices()
        {
            return new BittrexPricingMonitorService(this);
        }

        public override decimal MakerFee => 0.0025m;
        public override decimal TakerFee => 0.0025m;
        public override List<WithdrawalFee> GetWithdrawalFees()
        {
            var response = Get<ResponseDto<List<CurrencyDto>>>(PrivateUrl, "/public/getcurrencies");
            var activeCurrencies = response.Result.Where(m => m.IsActive);
            var withdrawalFees = new BuilderFactory().CreateCollection<CurrencyDto, WithdrawalFee>(activeCurrencies).Build().ToList();
            return withdrawalFees;
        }

        public override decimal GetBalances(string symbol)
        {
            var response = Get<dynamic>(PrivateUrl, "/account/getbalance", new Dictionary<string, object>
            {
                { "currency", symbol },
                { "apikey", Key }
            });
            var account = response.result;
            //var account = ((IEnumerable)response.result).Cast<dynamic>().FirstOrDefault(m => m.currency == symbol);
            return (decimal)(account?.Available?.Value ?? 0m);
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var markets = Get<ResponseDto<List<MarketSummaryDto>>>(PublicUrl, "/public/getmarketsummaries").Result;
            var activeCurrencies = Get<ResponseDto<List<CurrencyDto>>>(PrivateUrl, "/public/getcurrencies").Result.Where(m=>m.IsActive).Select(m=>m.Currency);
            var products = markets.Where(m=>eligibleSymbols.Contains(m.MarketCurrency) && activeCurrencies.Contains(m.MarketCurrency)).OrderByDescending(m => m.BaseVolume).ThenByDescending(m => m.Volume).ToList();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.MarketCurrency);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.MarketCurrency };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.BaseCurrency });
            }
        }
    }
}