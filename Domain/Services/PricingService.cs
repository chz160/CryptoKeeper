using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.DataObjects.Params;
using CryptoKeeper.Domain.Extensions;
using CryptoKeeper.Domain.Services.Apis;
using CryptoKeeper.Domain.Services.Factories;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public sealed class PricingService
    {
        private readonly object lockObject = new object();
        private static readonly Lazy<PricingService> lazy = new Lazy<PricingService>(() => new PricingService());
        private readonly ConcurrentDictionary<string, List<PricingItem>> _coinPricing = new ConcurrentDictionary<string, List<PricingItem>>();
        private readonly ConcurrentDictionary<string, List<WithdrawalFee>> _withdrawalFees = new ConcurrentDictionary<string, List<WithdrawalFee>>();
        private readonly IList<Thread> _threads;
        private readonly ICryptoCompareDataService _cryptoCompareDataService;
        private readonly IMathService _mathService;
        private readonly IBuilderFactory _builderFactory;

        public static PricingService Instance => lazy.Value;

        private PricingService() : this(new CryptoCompareDataService(), new MathService(), new BuilderFactory())
        { }

        private PricingService(ICryptoCompareDataService cryptoCompareDataService, IMathService mathService, IBuilderFactory builderFactory)
        {
            _threads = new List<Thread>();
            _builderFactory = builderFactory;
            _cryptoCompareDataService = cryptoCompareDataService;
            _mathService = mathService;
        }

        public HistoMinuteList GetPriceHistory(string exchange, string fromSymbol, string toSymbol, long timestamp)
        {
            var key = $"{exchange}_{fromSymbol}_{toSymbol}";
            var pricingList = _cryptoCompareDataService.GetCoinHistoryForExchange(exchange, fromSymbol, toSymbol, timestamp);
            _coinPricing[key] = _builderFactory.CreateCollection<HistoMinuteItem, PricingItem>(pricingList.Data).Build().ToList();
            return pricingList;
        }

        public HistoMinuteItem GetCurrentPrice(string exchange, string fromSymbol, string toSymbol)
        {
            //var key = $"{exchange}_{fromSymbol}_{toSymbol}";
            var histoMinuteList = _cryptoCompareDataService.GetCoinForExchange(exchange, fromSymbol, toSymbol);
            var pricingItems = _builderFactory.CreateCollection<HistoMinuteItem, PricingItem>(histoMinuteList).Build().ToList();
            UpdatePricingForMinute(exchange, fromSymbol, toSymbol, pricingItems);
            return histoMinuteList.FirstOrDefault();
        }

        public List<PricingItem> GetPricingForLast24Hours(string exchange, string fromSymbol, string toSymbol, long timestamp)
        {
            var key = $"{exchange}_{fromSymbol}_{toSymbol}";
            var timestamp24InThePast = DateTimeOffset.FromUnixTimeSeconds(timestamp).AddHours(-24).ToUnixTimeSeconds();
            return _coinPricing[key]
                .Where(m => 
                    m.Timestamp >= timestamp24InThePast && 
                    m.Timestamp <= timestamp)
                .OrderByDescending(m => m.Timestamp)
                .ToList();
        }

        public void UpdatePricingForMinute(string exchange, string fromSymbol, string toSymbol, PricingItem item)
        {
            UpdatePricingForMinute(exchange, fromSymbol, toSymbol, new List<PricingItem> {item});
        }

        public void UpdatePricingForMinute(string exchange, string fromSymbol, string toSymbol, List<PricingItem> items)
        {
            var key = $"{exchange}_{fromSymbol}_{toSymbol}";
            if (_coinPricing.ContainsKey(key) && _coinPricing[key] != null)
            {
                lock (lockObject)
                {
                    var existingRows = items.Where(m => _coinPricing[key].Select(n => n.Timestamp).Contains(m.Timestamp));
                    foreach (var existingRow in existingRows)
                    {
                        //var pricingItem = _builderFactory.Create<HistoMinuteItem, PricingItem>(existingRow).Build();
                        var replaceableItem = _coinPricing[key].First(m => m.Timestamp == existingRow.Timestamp);
                        _coinPricing[key].Replace(replaceableItem, existingRow);

                    }
                    var newRows = items.Where(m => !_coinPricing[key].Select(n => n.Timestamp).Contains(m.Timestamp)).ToList();
                    if (newRows.Any())
                    {
                        //var pricingItems = _builderFactory.CreateCollection<HistoMinuteItem, PricingItem>(newRows).Build();
                        _coinPricing[key].AddRange(newRows);
                    }
                }
            }
        }

        private decimal hightestDiffSeen = 0.01m;
        private decimal lowestDiffSeen = -0.01m;

        private decimal hightestPerSeen = 0.02m;
        private decimal lowestPerSeen = -0.01m;

        //Add transfer time to the weight. Shorter times is better.
        public decimal CalculateTrackbackWeight(string fromExchange, string toExchange, string fromSymbol, string toSymbol, decimal trackBackPercentDiff)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var fromHistory = GetPricingForLast24Hours(fromExchange, fromSymbol, toSymbol, timestamp);
            var toHistory = GetPricingForLast24Hours(toExchange, fromSymbol, toSymbol, timestamp);
            var endTime = DateTimeOffset.UtcNow;
            var startTime = endTime.AddHours(-24);
            var segmentFromData = fromHistory
                .Where(m =>
                    m.Timestamp <= endTime.ToUnixTimeSeconds() &&
                    m.Timestamp > startTime.ToUnixTimeSeconds()).ToList();
            var averagePriceFrom = segmentFromData.Sum(m => m.Price) / segmentFromData.Count * 100;
            var segmentToData = toHistory
                .Where(m =>
                    m.Timestamp <= endTime.ToUnixTimeSeconds() &&
                    m.Timestamp > startTime.ToUnixTimeSeconds()).ToList();
            var averagePriceTo = segmentToData.Sum(m => m.Price) / segmentToData.Count * 100;
            var percentDiff = _mathService.PercentDiff(averagePriceFrom, averagePriceTo);
            //percentage = 1 through 5
            var perWeight = _mathService.FindNumbersPosition(trackBackPercentDiff, lowestPerSeen, hightestPerSeen, 5);
            //diff % = 1 through 5
            var diffWeight = _mathService.FindNumbersPosition(percentDiff, lowestDiffSeen, hightestDiffSeen, 5);
            var chaos = perWeight + diffWeight;
            return chaos;
        }

        public void StartPricingDataThreads(ExchangePairParam exchangePairParam)
        {
            Console.Write("Getting historical data from exchanges...");
            var counter = 0;
            foreach (var exchange in exchangePairParam.Exchanges)
            {
                counter++;
                foreach (var coin in exchange.Coins.Where(m=> !exchangePairParam.ValueCoin.Contains(m.Symbol)))
                {
                    foreach (var childCoin in coin.Coins.Where(m=> 
                        (coin.Symbol == exchangePairParam.PrimaryCoin && exchangePairParam.ValueCoin.Contains(m.Symbol)) ||
                        (coin.Symbol != exchangePairParam.PrimaryCoin && m.Symbol == exchangePairParam.PrimaryCoin)))
                    {
                        //Console.WriteLine($"{exchange.Name}_{coin.Symbol}_{childCoin.Symbol}");
                        GetPriceHistory(exchange.Name, coin.Symbol, childCoin.Symbol, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
                    }
                }
                var thread = new Thread(() => new ExchangeApiServiceFactory().Create(exchange).MonitorPrices().Monitor());
                thread.IsBackground = true;
                thread.Name = string.Format("PricingThread{0}", counter);
                _threads.Add(thread);
                thread.Start();
            }
            Console.WriteLine("Done.");
        }

        private void DownloadPricingEveryMinute(Exchange exchange)
        {
            while (true)
            {
                Thread.Sleep(60000);
                foreach (var coin in exchange.Coins)
                {
                    foreach (var childCoin in coin.Coins)
                    {
                        //Console.WriteLine($"Thread {exchange.Name}_{coin.Symbol}_{childCoin.Symbol}");
                        GetCurrentPrice(exchange.Name, coin.Symbol, childCoin.Symbol);
                    }
                }
            }
        }

        public void UpdateExchangeCoinPrices(List<Exchange> exchanges)
        {
            UpdateExchangeCoinPrices(exchanges, null, null);
        }

        public void UpdateExchangeCoinPrices(List<Exchange> allExchanges, List<string> exchangeNames, List<string> symbols)
        {
            Console.Write("Updating exchange prices for coins...");
            var exchanges = exchangeNames == null ? allExchanges : allExchanges.Where(m => exchangeNames.Contains(m.Name));
            foreach (var exchange in exchanges)
            {
                var coins = symbols == null ? exchange.Coins : exchange.Coins.Where(m => symbols.Contains(m.Symbol));
                foreach (var coin in coins)
                {
                    foreach (var childCoin in coin.Coins)
                    {
                        var key = $"{exchange.Name}_{coin.Symbol}_{childCoin.Symbol}";
                        if (_coinPricing.ContainsKey(key) && _coinPricing[key].Any())
                        {
                            var pricingItem = _coinPricing[key].ToList().OrderByDescending(m => m.Timestamp).First();
                            childCoin.Price = pricingItem.Price;
                        }
                    }
                }
            }
            Console.WriteLine("Done.");
        }

        public List<WithdrawalFee> GetWithdrawalFeesForExchange(IAmAnApiService api)
        {
            if (!_withdrawalFees.ContainsKey(api.Name))
            {
                _withdrawalFees[api.Name] = new List<WithdrawalFee>();
            }

            if (_withdrawalFees[api.Name] == null || !_withdrawalFees[api.Name].Any())
            {
                _withdrawalFees[api.Name] = api.GetWithdrawalFees();
            }
            return _withdrawalFees[api.Name];
        }

        public decimal GetWithdrawalFeesForExchangeAndSymbol(IAmAnApiService api, string symbol)
        {
            if (api == null) throw new Exception("Api cannot be null.");
            var withdrawalFees = GetWithdrawalFeesForExchange(api);
            if (withdrawalFees == null) throw new Exception($"No withdrawal fees found for {api.Name}.");
            if (withdrawalFees.Any() && withdrawalFees.All(m => m.Symbol != symbol)) throw new Exception($"Cannot find withdrawal fees for {symbol} on {api.Name}.");
            return withdrawalFees.First(m => m.Symbol == symbol).Fee;
        }

        public decimal GetWithdrawalFeesForExchangeAndSymbol(Exchange exchange, string symbol)
        {
            var api = new ExchangeApiServiceFactory().Create(exchange);
            return GetWithdrawalFeesForExchangeAndSymbol(api, symbol);
        }

        public decimal GetTakerFeeForExchange(Exchange exchange)
        {
            var api = new ExchangeApiServiceFactory().Create(exchange);
            return api.TakerFee;
        }
    }
}