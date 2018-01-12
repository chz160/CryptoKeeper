using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class YobitPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public YobitPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public YobitPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            //while (true)
            //{
            //    //var response = _apiService.Get<ResponseDto<List<MarketSummaryDto>>>(_apiService.PublicUrl, "/public/getmarketsummaries");
            //    //var products = response.Result.OrderByDescending(m => m.BaseVolume).ThenByDescending(m => m.Volume).ToList();
            //    //foreach (var product in products)
            //    //{
            //    //    var pricingItem = _builderFactory.Create<MarketSummaryDto, PricingItem>(product).Build();
            //    //    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.Yobit, product.MarketCurrency, product.BaseCurrency, pricingItem);
            //    //}
            //    Thread.Sleep(60000);
            //}

            //while (true)
            //{
            //    Thread.Sleep(60000);
            //    foreach (var coin in _exchange.Coins)
            //    {
            //        foreach (var childCoin in coin.Coins)
            //        {
            //            PricingService.Instance.GetCurrentPrice(_exchange.Name, coin.Symbol, childCoin.Symbol);
            //        }
            //    }
            //}
        }

        //private void DownloadPricingEveryMinute(Exchange exchange)
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(60000);
        //        PricingService.Instance.
        //        foreach (var coin in exchange.Coins)
        //        {
        //            foreach (var childCoin in coin.Coins)
        //            {
        //                //Console.WriteLine($"Thread {exchange.Name}_{coin.Symbol}_{childCoin.Symbol}");
        //                GetCurrentPrice(exchange.Name, coin.Symbol, childCoin.Symbol);
        //            }
        //        }
        //    }
        //}
    }
}