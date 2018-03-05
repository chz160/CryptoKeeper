using System.Collections.Generic;
using System.Linq;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Gatecoin;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class GatecoinPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public GatecoinPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public GatecoinPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            var eligibleSymbols = _exchange.Coins.Select(m => m.Symbol)
                .Union(_exchange.Coins.SelectMany(m => m.Coins).Select(m => m.Symbol)).Distinct().ToList();
            var eligibleCombinedSymbols = new List<SymbolDto>();
            foreach (var symbol1 in eligibleSymbols)
            {
                foreach (var symbol2 in eligibleSymbols.Where(m => m != symbol1))
                {
                    eligibleCombinedSymbols.Add(new SymbolDto($"{symbol1}{symbol2}", symbol1, symbol2));
                }
            }

            var tickers = _apiService.Get<ResponseDto<List<TickerDto>>>(_apiService.PublicUrl, "/LiveTickers").tickers;
            tickers = tickers.Where(m => eligibleCombinedSymbols.Select(n=>n.Combined).Contains(m.currencyPair)).ToList();
            foreach (var ticker in tickers)
            {
                var symbolDto = eligibleCombinedSymbols.FirstOrDefault(m => m.Combined == ticker.currencyPair);
                if (symbolDto != null)
                {
                    var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(ticker).Build();
                    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.TrustDex, symbolDto.From, symbolDto.To, pricingItem);
                }
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