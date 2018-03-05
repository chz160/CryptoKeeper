using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Abucoins;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class AbucoinsPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public AbucoinsPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public AbucoinsPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            //while (true)
            //{
                foreach (var coin in _exchange.Coins)
                {
                    foreach (var childCoin in coin.Coins)
                    {
                        var product = _apiService.Get<TickerDto>(_apiService.PublicUrl, $"/products/{coin.Symbol}-{childCoin.Symbol}/ticker");
                        if (product != null)
                        {
                            var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(product).Build();
                            PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.Abucoins, coin.Symbol, childCoin.Symbol, pricingItem);
                        }
                    }
                }
            //    Thread.Sleep(60000);
            //}
        }
    }
}