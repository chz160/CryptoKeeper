using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BXinth;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BXinthPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public BXinthPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public BXinthPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            //while (true)
            //{
                var markets = _apiService.Get<Dictionary<string, MarketDto>>(_apiService.PublicUrl, "/").Select(m=>m.Value).ToList();
                foreach (var market in markets)
                {
                    var pricingItem = _builderFactory.Create<MarketDto, PricingItem>(market).Build();
                    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.BXinth, market.secondary_currency, market.primary_currency, pricingItem);
                }
            //    Thread.Sleep(60000);
            //}
        }
    }
}