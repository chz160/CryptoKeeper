using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BitBay;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BitBayPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public BitBayPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public BitBayPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            while (true)
            {
                foreach (var coin in _exchange.Coins)
                {
                    foreach (var childCoin in coin.Coins)
                    {
                        var ticker = _apiService.Get<TickerDto>(_apiService.PublicUrl, $"/{coin.Symbol}{childCoin.Symbol}/ticker.json");
                        var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(ticker).Build();
                        PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.BitBay, coin.Symbol, childCoin.Symbol, pricingItem);
                    }
                }
                Thread.Sleep(60000);
            }
        }
    }
}