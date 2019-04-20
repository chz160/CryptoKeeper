using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.WavesDex;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class WavesDexPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public WavesDexPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
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
                    _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.WavesDex, product.FromSymbol, product.ToSymbol, pricingItem);
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"WavesDex Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }
    }
}