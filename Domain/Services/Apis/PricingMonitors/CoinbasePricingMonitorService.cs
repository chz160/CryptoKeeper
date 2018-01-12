using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Coinbase;
using CryptoKeeper.Domain.Services.Interfaces;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class CoinbasePricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly IBuilderFactory _builderFactory;

        public CoinbasePricingMonitorService(IAmAnApiService apiService) : this(apiService, new BuilderFactory())
        { }

        public CoinbasePricingMonitorService(IAmAnApiService apiService, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            try
            {
                var socket = new ClientWebSocket();
                var task = socket.ConnectAsync(new Uri("wss://ws-feed.gdax.com"), CancellationToken.None);
                task.Wait();
                var readThread = new Thread(() => MessageRecived(socket)) { IsBackground = true };
                readThread.Start();
                var json = "{\"type\":\"subscribe\",\"channels\":[{\"name\":\"ticker\", \"product_ids\": [" + BuildProductIdString() + "]}]}";
                var bytes = Encoding.UTF8.GetBytes(json);
                var subscriptionMessageBuffer = new ArraySegment<byte>(bytes);
                socket.SendAsync(subscriptionMessageBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {

            }
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

        private void MessageRecived(ClientWebSocket socket)
        {
            var recBytes = new byte[1024];
            while (socket.State == WebSocketState.Open)
            {
                var t = new ArraySegment<byte>(recBytes);
                var receiveAsync = socket.ReceiveAsync(t, CancellationToken.None);
                receiveAsync.Wait();
                var jsonString = Encoding.UTF8.GetString(recBytes);
                var items = JsonConvert.DeserializeObject<TickerChannelDto>(jsonString);
                if (items.Type == "ticker")
                {
                    var pricingItem = _builderFactory.Create<TickerChannelDto, PricingItem>(items).Build();
                    var fromSymbol = items.Product_Id.Split("-")[0];
                    var toSymbol = items.Product_Id.Split("-")[1];
                    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.Coinbase, fromSymbol, toSymbol, pricingItem);
                }
                recBytes = new byte[1024];
            }
        }
    }
}