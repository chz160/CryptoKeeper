using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Poloniex;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Entities.Pricing.Models;

namespace CryptoKeeper.Domain.Services.Apis
{
    //Docs https://www.poloniex.com/support/api/
    //TODO: nonce parameter is required on POST
    public class PoloniexApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public PoloniexApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.Poloniex;
        public override string PublicUrl => "https://poloniex.com/public";
        public override string PrivateUrl => "https://poloniex.com/tradingApi";
        public override PricingApiType PricingApiType => PricingApiType.Rest;

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
            return new PoloniexPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override decimal MakerFee => 0.0025m;
        public override decimal TakerFee => 0.0025m;
        public override List<WithdrawalFee> GetWithdrawalFees()
        {
            var response = Get<Dictionary<string, CurrencyDto>>(PublicUrl, "?command=returnCurrencies");
            var currencies = response.Where(m => m.Value.Disabled == false && m.Value.Delisted == false && m.Value.Frozen == false);
            var withdrawalFees = new BuilderFactory().CreateCollection<KeyValuePair<string, CurrencyDto>, WithdrawalFee>(currencies).Build().ToList();
            return withdrawalFees;
        }

        public override decimal GetBalances(string symbol)
        {
            var accounts = Post<dynamic>(PrivateUrl, null, new Dictionary<string, object> { { "command", "returnCompleteBalances" } });
            var account = accounts[symbol];
            return decimal.Parse(account?.available?.Value ?? "0");
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var response = Get<Dictionary<string, CurrencyDto>>(PublicUrl, "?command=returnCurrencies");
            var products = response.Where(m => eligibleSymbols.Contains(m.Key) && m.Value.Delisted == false && m.Value.Frozen == false && m.Value.Disabled == false);
            var orderBook = GetOrderBook();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.Key);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.Key };
                    foreach (var order in orderBook.Where(m=>m.FromSymbol == product.Key && m.IsFrozenBool == false))
                    {
                        coin.Coins.Add(new Coin{Symbol = order.ToSymbol});
                    }
                    exchange.Coins.Add(coin);
                }
                //coin.Coins.Add(new Coin { Symbol = product.Key == SymbolConstants.Btc ? SymbolConstants.Usd : SymbolConstants.Btc });
            }
        }

        private List<OrderBookDto> GetOrderBook()
        {
            var result = new List<OrderBookDto>();
            var response = Get<Dictionary<string, OrderBookDto>>(PublicUrl, "?command=returnOrderBook&currencyPair=All&depth=10");
            foreach (var entry in response)
            {
                var market = entry.Key.Split("_");
                entry.Value.FromSymbol = market[0];
                entry.Value.ToSymbol = market[1];
                result.Add(entry.Value);
            }
            return result;
        }
    }
}