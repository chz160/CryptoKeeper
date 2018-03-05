﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class GeminiApiService : ApiService
    {
        private readonly Exchange _exchange;

        public GeminiApiService(Exchange exchange)
        {
            _exchange = exchange;
        }

        public override string Name => ExchangeConstants.Gemini;
        public override string PublicUrl => "https://api.gemini.com";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;
        
        public override IAmPricingMonitor MonitorPrices()
        {
            return new GeminiPricingMonitorService(this, _exchange);
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var eligibleCombinedSymbols = new List<SymbolDto>();
            foreach (var symbol1 in eligibleSymbols)
            {
                foreach (var symbol2 in eligibleSymbols.Where(m => m != symbol1))
                {
                    eligibleCombinedSymbols.Add(new SymbolDto($"{symbol1}{symbol2}", symbol1, symbol2));
                }
            }

            var markets = Get<List<string>>(PublicUrl, "/v1/symbols");
            markets = markets.Where(m => eligibleCombinedSymbols.Select(n => n.Combined).Contains(m.ToUpper())).ToList();
            foreach (var market in markets)
            {
                var symbolDto = eligibleCombinedSymbols.First(m => m.Combined == market.ToUpper());
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == symbolDto.From);
                if (coin == null)
                {
                    coin = new Coin { Symbol = symbolDto.From };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = symbolDto.To });
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