using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Exmo;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class ExmoPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public ExmoPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public ExmoPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            //while (true)
            //{
                var tickers = _apiService.Get<Dictionary<string, TickerDto>>(_apiService.PublicUrl, "/ticker");
                foreach (var ticker in tickers)
                {
                    ticker.Value.from_symbol = ticker.Key.Split("_")[0];
                    ticker.Value.to_symbol = ticker.Key.Split("_")[1];
                }
                var products = tickers.Select(m => m.Value).ToList();
                var validProducts = products.Where(m =>
                    !SymbolConstants.FiatCurrency.Contains(m.from_symbol) &&
                    !SymbolConstants.FiatCurrency.Contains(m.to_symbol)).ToList();
                foreach (var product in validProducts)
                {
                    var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(product).Build();
                    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.Exmo, product.from_symbol, product.to_symbol, pricingItem);
                }
            //    Thread.Sleep(60000);
            //}
        }
    }
}