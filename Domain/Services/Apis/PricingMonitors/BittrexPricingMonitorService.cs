using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.BitTrex;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BittrexPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public BittrexPricingMonitorService(IAmAnApiService apiService, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _builderFactory = builderFactory;
            _serviceProvider = serviceProvider;
        }

        public void Monitor()
        {
            try
            {
                //Should probably also use the getticker endpoint as the data is not cached as long and more up-to-date.
                var response = _apiService.Get<ResponseDto<List<MarketSummaryDto>>>(_apiService.PublicUrl, "/public/getmarketsummaries");
                var products = response.Result.OrderByDescending(m => m.BaseVolume).ThenByDescending(m => m.Volume).ToList();
                foreach (var product in products)
                {
                    var pricingItem = _builderFactory.Create<MarketSummaryDto, PricingItem>(product).Build();
                    _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.BitTrex, product.MarketCurrency, product.BaseCurrency, pricingItem);
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Bittrex Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }
    }
}