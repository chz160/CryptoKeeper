using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Exmo;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class ExmoPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public ExmoPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
            _serviceProvider = serviceProvider;
        }

        public void Monitor()
        {
            try
            {
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
                    _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.Exmo, product.from_symbol, product.to_symbol, pricingItem);
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Exmo Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }
    }
}