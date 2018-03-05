using System.Linq;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Exx;
using CryptoKeeper.Domain.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class ExxPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;

        public ExxPricingMonitorService(IAmAnApiService apiService, Exchange exchange) : this(apiService, exchange, new BuilderFactory())
        { }

        public ExxPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory)
        {
            _apiService = apiService;
            _exchange = exchange;
            _builderFactory = builderFactory;
        }

        public void Monitor()
        {
            var eligibleSymbols = _exchange.Coins.Select(m => m.Symbol)
                .Union(_exchange.Coins.SelectMany(m => m.Coins).Select(m => m.Symbol)).Distinct().ToList();
            //while (true)
            //{
                var response = _apiService.Get<JObject>(_apiService.PublicUrl, "/tickers");
                var tickers = new BuilderFactory().CreateCollection<JToken, TickerDto>(response).Build();
                tickers = tickers.Where(m =>
                    eligibleSymbols.Contains(m.fromSymbol) &&
                    eligibleSymbols.Contains(m.toSymbol)).ToList();
                foreach (var ticker in tickers)
                {
                    var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(ticker).Build();
                    PricingService.Instance.UpdatePricingForMinute(ExchangeConstants.TrustDex, ticker.fromSymbol, ticker.toSymbol, pricingItem);
                }
            //    Thread.Sleep(60000);
            //}
        }
    }
}