﻿using System;
using System.Drawing;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Bitstamp;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BitstampPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public BitstampPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
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
                        var ticker = _apiService.Get<TickerDto>(_apiService.PublicUrl, $"/ticker/{coin.Symbol.ToLower()}{childCoin.Symbol.ToLower()}");
                        var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(ticker).Build();
                        _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.Bitstamp, coin.Symbol, childCoin.Symbol, pricingItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Bitstamp Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }
    }
}