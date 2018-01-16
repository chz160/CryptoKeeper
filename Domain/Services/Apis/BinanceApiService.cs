using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Binance;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class BinanceApiService : ApiService
    {
        private readonly Exchange _exchange;

        public BinanceApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.Binance;
        public override string PublicUrl => "https://api.binance.com/api";
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
            return new BinancePricingMonitorService(this, _exchange);
        }

        //Binance has no withdrawal fees right now. Set everthing to 0.
        public override List<WithdrawalFee> GetWithdrawalFees()
        {
            var result = new List<WithdrawalFee>();
            foreach (var coin in _exchange.Coins)
            {
                result.Add(new WithdrawalFee { Symbol = coin.Symbol, Fee = 0m });
            }
            return result;
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<ResponseDto>(PublicUrl, "/v1/exchangeInfo");
            if (response != null)
            {
                var products = response.symbols.Where(m => eligibleSymbols.Contains(m.baseAsset) && eligibleSymbols.Contains(m.quoteAsset) && m.status == "TRADING");
                foreach (var product in products)
                {
                    var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.baseAsset);
                    if (coin == null)
                    {
                        coin = new Coin { Symbol = product.baseAsset };
                        exchange.Coins.Add(coin);
                    }
                    coin.Coins.Add(new Coin { Symbol = product.quoteAsset });
                }
            }
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}