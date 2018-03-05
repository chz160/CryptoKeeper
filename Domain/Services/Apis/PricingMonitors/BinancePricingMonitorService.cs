using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Binance;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Domain.Utilities;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class BinancePricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private Dictionary<string, string> _channels;

        public BinancePricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public BinancePricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            try
            {
                var channels = BuildChannels();
                var socket = WebSocketWrapper.Create($"wss://stream.binance.com:9443/ws{channels}");
                socket.OnMessage(OnMessage);
                socket.OnDisconnect(OnDisconnect);
                socket.Connect();
            }
            catch (Exception ex)
            {

            }
        }

        private void OnMessage(string message, WebSocketWrapper wrapper)
        {
            if (!string.IsNullOrEmpty(message) && message.Contains("\"e\":\"24hrTicker\""))
            {
                var items = JsonConvert.DeserializeObject<TickerDto>(message);
                var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(items).Build();
                var fromSymbol = _channels[items.Symbol.ToUpper()].Split(",")[0];
                var toSymbol = _channels[items.Symbol.ToUpper()].Split(",")[1];
                PricingService.Instance.UpdatePricingForMinute(_apiService.Name, fromSymbol, toSymbol, pricingItem);
            }
        }

        private void OnDisconnect(WebSocketWrapper wrapper)
        {
            Colorful.Console.WriteLine($"{_apiService.Name} pricing monitor disconnected from web socket. Reconnecting...", Color.Red);
            Monitor();
        }

        private string BuildChannels()
        {
            _channels = new Dictionary<string, string>();
            foreach (var coin in _exchange.Coins)
            {
                foreach (var childCoin in coin.Coins)
                {
                    var symbol = coin.Symbol + childCoin.Symbol;
                    var values = $"{coin.Symbol},{childCoin.Symbol}";
                    _channels.Add(symbol, values);
                }
            }
            return string.Join("", _channels.Keys.Select(m=>$"/{m.ToLower()}@ticker"));
        }
    }
}