using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.WavesDex;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class WavesDexPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public WavesDexPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public WavesDexPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            while (true)
            {
                var response = _apiService.Get<List<TickerDto>>(_apiService.PublicUrl, "/tickers");
                var symbolList = _exchange.Coins.Select(n => n.Symbol)
                    .Union(_exchange.Coins.SelectMany(n => n.Coins).Select(n => n.Symbol)).ToList();
                var products = response
                    .OrderByDescending(m => m.Volume24Hour)
                    .Where(m =>
                        symbolList.Contains(m.FromSymbol) &&
                        symbolList.Contains(m.ToSymbol)).ToList();
                foreach (var product in products)
                {
                    var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(product).Build();
                    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.WavesDex, product.FromSymbol, product.ToSymbol, pricingItem);
                }
                Thread.Sleep(60000);
            }
        }
    }
}