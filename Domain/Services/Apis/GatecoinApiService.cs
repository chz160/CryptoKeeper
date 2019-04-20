using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Gatecoin;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class GatecoinApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public GatecoinApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.Gatecoin;
        public override string PublicUrl => "https://api.gatecoin.com/public";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new GatecoinPricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            try
            {
                var eligibleCombinedSymbols = new List<SymbolDto>();
                foreach (var symbol1 in eligibleSymbols)
                {
                    foreach (var symbol2 in eligibleSymbols.Where(m => m != symbol1))
                    {
                        eligibleCombinedSymbols.Add(new SymbolDto($"{symbol1}{symbol2}", symbol1, symbol2));
                    }
                }

                var markets = Get<ResponseDto<List<TickerDto>>>(PublicUrl, "/LiveTickers").tickers;
                markets = markets.Where(m => eligibleCombinedSymbols.Select(n => n.Combined).Contains(m.currencyPair)).ToList();
                foreach (var market in markets)
                {
                    var symbolDto = eligibleCombinedSymbols.First(m => m.Combined == market.currencyPair);
                    var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == symbolDto.From);
                    if (coin == null)
                    {
                        coin = new Coin { Symbol = symbolDto.From };
                        exchange.Coins.Add(coin);
                    }
                    coin.Coins.Add(new Coin { Symbol = symbolDto.To });
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Gatecoin GetProducts: {ex.Message}\r\n{ex.Source}", Color.Red);
            }
        }

        class SymbolDto
        {
            public SymbolDto(string combined, string from, string to)
            {
                Combined = combined;
                From = from;
                To = to;
            }
            public string Combined { get; set; }
            public string From { get; set; }
            public string To { get; set; }
        }
    }
}