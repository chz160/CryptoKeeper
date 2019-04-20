using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Yobit;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Entities.Pricing.Models;

namespace CryptoKeeper.Domain.Services.Apis
{
    //Docs https://www.yobit.net/en/api/
    public class YobitApiService : ApiService
    {
        private readonly Exchange _exchange;

        public YobitApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.Yobit;
        public override string PublicUrl => "https://yobit.net/api/3";
        public override string PrivateUrl => "https://yobit.net/tapi";
        public override PricingApiType PricingApiType => PricingApiType.CryptoCompare;
        public override string ContentType => "application/x-www-form-urlencoded";

        public override HMAC GetHMac()
        {
            return new HMACSHA512(Encoder.GetBytes(Secret));
        }

        public override bool SignAsHex => true;

        protected override void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body)
        {
            request.Headers.Add("Key", Key);
            request.Headers.Add("Sign", SignString(body));
        }

        public override IAmPricingMonitor MonitorPrices()
        {
            return new YobitPricingMonitorService(this, _exchange);
        }

        public override decimal MakerFee => 0.002m;
        public override decimal TakerFee => 0.002m;
        public override List<WithdrawalFee> GetWithdrawalFees()
        {
            return new List<WithdrawalFee>{
                new WithdrawalFee {Symbol = "BTC", Fee = 0.0012m},
                new WithdrawalFee {Symbol = "LTC", Fee = 0.002m},
                new WithdrawalFee {Symbol = "ETH", Fee = 0.005m},
                new WithdrawalFee {Symbol = "ETC", Fee = 0.005m},
                new WithdrawalFee {Symbol = "XVG", Fee = 0.002m},
                new WithdrawalFee {Symbol = "NMC", Fee = 0.002m},
                new WithdrawalFee {Symbol = "PPC", Fee = 0.2m},
                new WithdrawalFee {Symbol = "DASH", Fee = 0.002m},
                new WithdrawalFee {Symbol = "VIA", Fee = 0.002m},
                new WithdrawalFee {Symbol = "DGB", Fee = 0.002m},
                new WithdrawalFee {Symbol = "ZEC", Fee = 0.002m}
            };
        }

        public override decimal GetBalances(string symbol)
        {
            var response = Post<dynamic>(PrivateUrl, null, new Dictionary<string, object> {{ "method", "getInfo" } });
            var account = response["return"]["funds"];
            //var account = ((IEnumerable)response.result).Cast<dynamic>().FirstOrDefault(m => m.currency == symbol);
            return decimal.Parse(account[symbol.ToLower()].Value.ToString() ?? 0m, NumberStyles.Float);
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<ResponseDto>(PublicUrl, "/info");
            var products = response.Pairs.Where(m => m.Value.Hidden == false);
            foreach (var product in products)
            {
                var marketCurrency = product.Key.Split("_")[0].ToUpper();
                if (eligibleSymbols.Contains(marketCurrency))
                {
                    var baseCurrency = product.Key.Split("_")[1].ToUpper();
                    var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == marketCurrency);
                    if (coin == null)
                    {
                        coin = new Coin { Symbol = marketCurrency };
                        exchange.Coins.Add(coin);
                    }
                    coin.Coins.Add(new Coin { Symbol = baseCurrency });
                }
            }
        }
    }
}