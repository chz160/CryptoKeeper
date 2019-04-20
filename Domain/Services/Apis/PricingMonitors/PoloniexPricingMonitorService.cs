using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Poloniex;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class PoloniexPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public PoloniexPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
            _serviceProvider = serviceProvider;
        }

        public void Monitor()
        {
            //try
            //{
            //    var socket = new ClientWebSocket();
            //    var task = socket.ConnectAsync(new Uri("wss://api.poloniex.com"), CancellationToken.None);
            //    task.Wait();
            //    var readThread = new Thread(() => MessageRecived(socket)) { IsBackground = true };
            //    readThread.Start();
            //    var json = "{\"type\":\"subscribe\",\"channels\":[{\"name\":\"ticker\", \"product_ids\": [" + BuildProductIdString() + "]}]}";
            //    var bytes = Encoding.UTF8.GetBytes(json);
            //    var subscriptionMessageBuffer = new ArraySegment<byte>(bytes);
            //    socket.SendAsync(subscriptionMessageBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            //}
            //catch (Exception ex)
            //{

            //}

            //while (true)
            //{
            //    Thread.Sleep(60000);
            //foreach (var coin in _exchange.Coins)
            //{
            //    foreach (var childCoin in coin.Coins)
            //    {

            //    }
            //}
            //}
            //PricingService.Instance.GetCurrentPrice(_exchange.Name, coin.Symbol, childCoin.Symbol);

            try
            {
                var response = _apiService.Get<Dictionary<string, TickerDto>>(_apiService.PublicUrl, "?command=returnTicker");
                foreach (var key in response.Keys)
                {
                    response[key].FromSymbol = key.Split("_")[1];
                    response[key].ToSymbol = key.Split("_")[0];
                }

                var products = response.Values.Where(m => m.IsFrozenBool == false).ToList();
                foreach (var product in products)
                {
                    if (product != null)
                    {
                        var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(product).Build();
                        _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.Poloniex, product.FromSymbol, product.ToSymbol, pricingItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Poloniex Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
        }
    }
}