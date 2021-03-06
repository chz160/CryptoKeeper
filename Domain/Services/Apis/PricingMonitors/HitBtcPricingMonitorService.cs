﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.HitBtc;
using CryptoKeeper.Domain.Services.Factories;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Domain.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class HitBtcPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<string, string> _symbolLookup;

        public HitBtcPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
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
                var socket = WebSocketWrapper.Create("wss://api.hitbtc.com/api/2/ws");
                socket.OnConnect(OnConnect);
                socket.OnMessage(OnMessage);
                socket.OnDisconnect(OnDisconnect);
                socket.Connect();
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"HitBtc Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }

        private void OnConnect(WebSocketWrapper wrapper)
        {
            _symbolLookup = new Dictionary<string, string>();
            foreach (var coin in _exchange.Coins)
            {
                foreach (var childCoin in coin.Coins)
                {
                    var symbol = coin.Symbol + childCoin.Symbol;
                    _symbolLookup.Add(symbol, $"{coin.Symbol},{childCoin.Symbol}");
                    var json = "{\"method\":\"subscribeTicker\",\"params\":{\"symbol\":\"" + symbol + "\"}, \"id\": 123}";
                    wrapper.SendMessage(json);
                }
            }
        }

        private void OnMessage(string message, WebSocketWrapper wrapper)
        {
            var item = JsonConvert.DeserializeObject<ResponseDto<TickerDto>>(message);
            if (item.Method == "ticker" && !string.IsNullOrEmpty(item.Params.Ask) && !string.IsNullOrEmpty(item.Params.Bid))
            {
                var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(item.Params).Build();
                if (_symbolLookup.ContainsKey(item.Params.Symbol))
                {
                    var symbolPair = _symbolLookup[item.Params.Symbol].Split(",");
                    var fromSymbol = symbolPair[0];
                    var toSymbol = symbolPair[1];
                    _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.HitBtc, fromSymbol, toSymbol, pricingItem);
                }
            }
        }

        private void OnDisconnect(WebSocketWrapper wrapper)
        {
            Colorful.Console.WriteLine($"{_apiService.Name} pricing monitor disconnected from web socket. Reconnecting...", Color.Red);
            Monitor();
        }
    }
}