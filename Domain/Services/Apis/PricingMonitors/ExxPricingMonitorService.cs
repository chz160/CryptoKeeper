using System;
using System.Drawing;
using System.Linq;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Exx;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class ExxPricingMonitorService : IAmPricingMonitor
    {
        private readonly IAmAnApiService _apiService;
        private readonly Exchange _exchange;
        private readonly IBuilderFactory _builderFactory;
        private readonly IServiceProvider _serviceProvider;

        public ExxPricingMonitorService(IAmAnApiService apiService, Exchange exchange, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
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
                var eligibleSymbols = _exchange.Coins.Select(m => m.Symbol)
                        .Union(_exchange.Coins.SelectMany(m => m.Coins).Select(m => m.Symbol)).Distinct().ToList();
                var response = _apiService.Get<JObject>(_apiService.PublicUrl, "/tickers");
                var tickers = new BuilderFactory().CreateCollection<JToken, TickerDto>(response).Build();
                tickers = tickers.Where(m =>
                    eligibleSymbols.Contains(m.fromSymbol) &&
                    eligibleSymbols.Contains(m.toSymbol)).ToList();
                foreach (var ticker in tickers)
                {
                    var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(ticker).Build();
                    _serviceProvider.GetRequiredService<IPricingService>().UpdatePricingForMinute(ExchangeConstants.TrustDex, ticker.fromSymbol, ticker.toSymbol, pricingItem);
                }
            }
            catch (Exception ex)
            {
                Colorful.Console.WriteLine($"Exx Monitor(): {ex.Message}\r\n{ex.StackTrace}", Color.Red);
            }
            
        }
    }
}