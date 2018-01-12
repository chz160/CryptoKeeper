using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class PoloniexPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public PoloniexPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public PoloniexPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
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
            //    foreach (var coin in _exchange.Coins)
            //    {
            //        foreach (var childCoin in coin.Coins)
            //        {
            //            PricingService.Instance.GetCurrentPrice(_exchange.Name, coin.Symbol, childCoin.Symbol);
            //        }
            //    }
            //}
        }
    }
}