using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Coinbase;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Domain.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class CoinbasePricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public CoinbasePricingMonitorService(IAmAnApiService apiService, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _builderFactory = builderFactory;
            _serviceProvider = serviceProvider;
        }

        public void Monitor()
        {
            try
            {
                var socket = WebSocketWrapper.Create("wss://ws-feed.gdax.com");
                socket.OnConnect(OnConnect);
                socket.OnMessage(OnMessage);
                socket.OnDisconnect(OnDisconnect);
                socket.Connect();
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Coinbase Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }

        private void OnConnect(WebSocketWrapper wrapper)
        {
            var json = "{\"type\":\"subscribe\",\"channels\":[{\"name\":\"ticker\", \"product_ids\": [" + BuildProductIdString() + "]}]}";
            wrapper.SendMessage(json);
        }

        private void OnMessage(string message, WebSocketWrapper wrapper)
        {
            if (string.IsNullOrEmpty(message)) return;
            var items = JsonConvert.DeserializeObject<TickerChannelDto>(message);
            if (items.Type == "ticker")
            {
                var pricingItem = _builderFactory.Create<TickerChannelDto, PricingItem>(items).Build();
                var fromSymbol = items.Product_Id.Split("-")[0];
                var toSymbol = items.Product_Id.Split("-")[1];
                _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.Coinbase, fromSymbol, toSymbol, pricingItem);
            }
        }

        private void OnDisconnect(WebSocketWrapper wrapper)
        {
            Colorful.Console.WriteLine("Coinbase pricing monitor disconnected from web socket. Reconnecting...", Color.Red);
            Monitor();
        }

        private string BuildProductIdString()
        {
            var ids = new List<string>();
            var products = _apiService.Get<List<ProductDto>>(_apiService.PublicUrl, "/products");
            foreach (var product in products.Where(m=>!m.Id.Contains("-GBP") && !m.Id.Contains("-EUR")))
            {
                ids.Add(product.Id);
            }
            return string.Join(",", ids.Select(m => $"\"{m}\""));
        }
    }
}