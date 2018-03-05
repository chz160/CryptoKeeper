using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BitTrex;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BittrexPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly IBuilderFactory _builderFactory;

        public BittrexPricingMonitorService(IAmAnApiService apiService) : this(apiService, new BuilderFactory())
        { }

        public BittrexPricingMonitorService(IAmAnApiService apiService, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            //while (true)
            //{
                //Should probably also use the getticker endpoint as the data is not cached as long and more up-to-date.
                var response = _apiService.Get<ResponseDto<List<MarketSummaryDto>>>(_apiService.PublicUrl, "/public/getmarketsummaries");
                var products = response.Result.OrderByDescending(m => m.BaseVolume).ThenByDescending(m => m.Volume).ToList();
                foreach (var product in products)
                {
                    var pricingItem = _builderFactory.Create<MarketSummaryDto, PricingItem>(product).Build();
                    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.BitTrex, product.MarketCurrency, product.BaseCurrency, pricingItem);
                }
            //    Thread.Sleep(60000);
            //}
        }
    }
}