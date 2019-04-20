using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BXinth;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BXinthPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public BXinthPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
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
                var markets = _apiService.Get<Dictionary<string, MarketDto>>(_apiService.PublicUrl, "/").Select(m => m.Value).ToList();
                foreach (var market in markets)
                {
                    var pricingItem = _builderFactory.Create<MarketDto, PricingItem>(market).Build();
                    _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.BXinth, market.secondary_currency, market.primary_currency, pricingItem);
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"BXinth Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }
    }
}