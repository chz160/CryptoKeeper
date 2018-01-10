using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Coinbase;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    //Docs https://docs.gdax.com/?javascript#api
    public class CoinbaseApiService : ApiService
    {
        public override string Name => ExchangeConstants.Coinbase;
        public override string PublicUrl => "https://api.gdax.com";
        public override string PrivateUrl => PublicUrl;
        public override PricingApiType PricingApiType => PricingApiType.WebSocket;
        public override bool RequiresNonce => false;

        public override HMAC GetHMac()
        {
            return new HMACSHA256(Convert.FromBase64String(Secret));
        }

        protected override void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            request.Headers.Add("CB-ACCESS-KEY", Key);
            request.Headers.Add("CB-ACCESS-TIMESTAMP", timestamp.ToString());
            request.Headers.Add("CB-ACCESS-PASSPHRASE", Passphrase);
            if (MustBeSigned)
            {
                var what = $@"{timestamp}GET{relativeUrl}{body}";
                var sig = SignString(what);
                request.Headers.Add("CB-ACCESS-SIGN", sig);
            }
        }

        public override IAmPricingMonitor MonitorPrices()
        {
            return new CoinbasePricingMonitorService(this);
        }

        public override decimal MakerFee => 0;
        public override decimal TakerFee => 0.0025m;
        public override List<WithdrawalFee> GetWithdrawalFees()
        {
            return new List<WithdrawalFee>{
                new WithdrawalFee {Symbol = "BTC", Fee = 0},
                new WithdrawalFee {Symbol = "LTC", Fee = 0},
                new WithdrawalFee {Symbol = "ETH", Fee = 0}
            };
        }

        public override long GetServerTime()
        {
            return ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public override decimal GetBalances(string symbol)
        {
            var accounts = Get<dynamic>(PrivateUrl, "/accounts");
            var account = ((IEnumerable) accounts).Cast<dynamic>().FirstOrDefault(m => m.currency == symbol);
            return decimal.Parse(account?.balance?.Value ?? "0");
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var products = Get<List<ProductDto>>(PublicUrl, "/products");
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.Base_Currency);
                if (coin == null)
                {
                    coin = new Coin {Symbol = product.Base_Currency};
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.Quote_Currency });
            }
        }
    }
}
