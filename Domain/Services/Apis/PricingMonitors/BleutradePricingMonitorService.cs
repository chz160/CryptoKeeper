using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Bleutrade;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BleutradePricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public BleutradePricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
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
                foreach (var coin in _exchange.Coins)
                {
                    foreach (var childCoin in coin.Coins)
                    {
                        try
                        {
                            var ticker = _apiService.Get<ResponseDto<List<TickerDto>>>(_apiService.PublicUrl, $"/getticker?market={coin.Symbol}_{childCoin.Symbol}").result.First();
                            var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(ticker).Build();
                            _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.Bleutrade, coin.Symbol, childCoin.Symbol, pricingItem);
                        }
                        catch (Exception e)
                        {
                            //Colorful.Console.WriteLine(e, Color.Red);
                        }
                        Task.Delay(500).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Bleutrade Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }
    }
}