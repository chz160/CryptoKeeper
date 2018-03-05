using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.DataObjects.Params;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Extensions;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Factories;
using CryptoKeeper.Domain.Services.Interfaces;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using ThreadState = System.Threading.ThreadState;

namespace CryptoKeeper.Domain.Services
{
    public sealed partial class PricingService
    {
        private Socket _cryptoCompareSocket;
        private readonly object lockObject = new object();
        private static readonly Lazy<PricingService> lazy = new Lazy<PricingService>(() => new PricingService());
        private readonly ConcurrentDictionary<string, List<PricingItem>> _coinPricing = new ConcurrentDictionary<string, List<PricingItem>>();
        private readonly ConcurrentDictionary<string, string[]> _currentCryptoCompareRows = new ConcurrentDictionary<string, string[]>();
        private readonly ConcurrentDictionary<string, List<WithdrawalFee>> _withdrawalFees = new ConcurrentDictionary<string, List<WithdrawalFee>>();
        private readonly IList<Thread> _threads;
        private readonly ICryptoCompareDataService _cryptoCompareDataService;
        private readonly IMathService _mathService;
        private readonly IBuilderFactory _builderFactory;
        private System.Timers.Timer _timer;

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

        public string[] GetCurrentPricingAsStringArray(string exchange, string fromSymbol, string toSymbol)
        {
            var key = $"{exchange}_{fromSymbol}_{toSymbol}";
            var currentRow = _coinPricing[key].OrderByDescending(m => m.Timestamp).FirstOrDefault();
            return new[] { null, null, null, null, null, currentRow?.Price.ToString(), currentRow?.Bid.ToString(), currentRow?.Ask.ToString(), currentRow?.Timestamp.ToString(), null, null, null, null, null, null, currentRow?.Volume.ToString(), null, null, null, null, null, null, null, null };
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
            lock (lockObject)
            {
                if (!_coinPricing.ContainsKey(key) || _coinPricing[key] == null)
                {
                    _coinPricing[key] = new List<PricingItem>();
                }
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
            //var counter = 0;
            foreach (var exchange in exchangePairParam.Exchanges)
            {
                //counter++;
                //foreach (var coin in exchange.Coins.Where(m => !SymbolConstants.FiatCurrency.Contains(m.Symbol) ))
                //{
                //    foreach (var childCoin in coin.Coins.Where(m => !SymbolConstants.FiatCurrency.Contains(m.Symbol)))
                //    {
                //        //Console.WriteLine($"{exchange.Name}_{coin.Symbol}_{childCoin.Symbol}");
                //        //GetPriceHistory(exchange.Name, coin.Symbol, childCoin.Symbol, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
                //    }
                //}

                var apiService = new ExchangeApiServiceFactory().Create(exchange);
                var priceMonitor = apiService.MonitorPrices();
                if (priceMonitor.GetType() != typeof(NullMonitorService) && apiService.PricingApiType == PricingApiType.WebSocket)
                {
                    //var threadName = $"PricingThreadFor{exchange.Name}";
                    //if (DoesThreadExist(threadName) == false)
                    //{
                    //    var thread = new Thread(() => priceMonitor.Monitor());
                    //    thread.IsBackground = true;
                    //    thread.Name = threadName;
                    //    _threads.Add(thread);
                    //    thread.Start();
                    //}
                    priceMonitor.Monitor();
                }
            }

            CheckpricingForRestfulExchanges(exchangePairParam.Exchanges);
            _timer = new System.Timers.Timer(1000 * 60);
            _timer.Elapsed += (sender, e) => CheckpricingForRestfulExchanges(exchangePairParam.Exchanges);
            _timer.Start();

            //var ccThreadName = $"PricingThreadForCryptoCompare";
            //if (DoesThreadExist(ccThreadName) == false)
            //{
            //    var cryptoThread = new Thread(() => ListenToCryptoCompareTicker(exchangePairParam.Exchanges));
            //cryptoThread.IsBackground = true;
            //cryptoThread.Name = ccThreadName;
            //_threads.Add(cryptoThread);
            //cryptoThread.Start();
            //}
            
            
            //ListenToCryptoCompareTicker(exchangePairParam.Exchanges);

            Console.WriteLine("Done.");

            var primeTimeout = DateTime.Now.AddMinutes(15);
            while (DateTime.Now < primeTimeout)
            {
                Console.Write($"Letting exchange data prime for {(primeTimeout - DateTime.Now):mm\\:ss}\r");
                Task.Delay(1000).Wait();
            }
            PrintPrimingReport(exchangePairParam.Exchanges);
            Console.WriteLine("Primed.");
        }

        private void PrintPrimingReport(List<Exchange> exchanges)
        {
            Console.WriteLine($"Priming complete.");
            Colorful.Console.WriteLine("--------------------------------------------------------------------------------------", Color.Yellow);
            foreach (var exchange in exchanges.OrderBy(m=>m.Name))
            {
                Colorful.Console.WriteLine(exchange.Name, Color.Yellow);
                var counter = 0;
                foreach (var coin in exchange.Coins)
                {
                    foreach (var childCoin in coin.Coins)
                    {
                        var key = $"{exchange.Name}_{coin.Symbol}_{childCoin.Symbol}";
                        if (_coinPricing.ContainsKey(key))
                        {
                            Colorful.Console.Write($"\t{coin.Symbol.PadRight(5)} -> {childCoin.Symbol.PadRight(5)}: {_coinPricing[key]?.Count ?? 0}", Color.Yellow);
                            counter++;
                            if (counter == 5)
                            {
                                counter = 0;
                                Colorful.Console.WriteLine("", Color.Yellow);
                            }
                        }
                    }
                }

                if (counter != 0)
                {
                    Colorful.Console.WriteLine("", Color.Yellow);
                }
            }
            Colorful.Console.WriteLine("--------------------------------------------------------------------------------------", Color.Yellow);
        }

        private void CheckpricingForRestfulExchanges(List<Exchange> exchanges)
        {
            foreach (var exchange in exchanges)
            {
                var apiService = new ExchangeApiServiceFactory().Create(exchange);
                var priceMonitor = apiService.MonitorPrices();
                if (priceMonitor.GetType() != typeof(NullMonitorService) && apiService.PricingApiType == PricingApiType.Rest)
                {
                    var threadName = $"PricingThreadFor{exchange.Name}";
                    if (DoesThreadExist(threadName) == false)
                    {
                        var thread = new Thread(() => priceMonitor.Monitor());
                        thread.IsBackground = true;
                        thread.Name = threadName;
                        //_threads.Add(thread);
                        thread.Start();
                    }
                }
            }
        }

        private bool DoesThreadExist(string threadName)
        {
            var thread = _threads.FirstOrDefault(m => m.Name == threadName);
            return thread != null && thread.IsAlive && (
                       thread.ThreadState.HasFlag(ThreadState.Running) || 
                       thread.ThreadState.HasFlag(ThreadState.WaitSleepJoin) || 
                       thread.ThreadState.HasFlag(ThreadState.Background));
            //return _threads.Any(m => m.Name == threadName && m.IsAlive && (m.ThreadState == ThreadState.Running || m.ThreadState == ThreadState.WaitSleepJoin));
        }
        //private void DownloadPricingEveryMinute(Exchange exchange)
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(60000);
        //        foreach (var coin in exchange.Coins)
        //        {
        //            foreach (var childCoin in coin.Coins)
        //            {
        //                //Console.WriteLine($"Thread {exchange.Name}_{coin.Symbol}_{childCoin.Symbol}");
        //                GetCurrentPrice(exchange.Name, coin.Symbol, childCoin.Symbol);
        //            }
        //        }
        //    }
        //}

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
                            var items = _coinPricing[key].OrderByDescending(m => m.Timestamp).ToList();
                            var now = items.First().Timestamp;
                            var backTo = DateTimeOffset.FromUnixTimeSeconds(now).AddMinutes(-5).ToUnixTimeSeconds();
                            var pricingItems = items.Where(m=>m.Timestamp <= now && m.Timestamp >= backTo).ToList();
                            var fiveMinutePrice = pricingItems.Sum(m => m.Price) / pricingItems.Count;
                            var fiveMinuteAsk = pricingItems.Sum(m => m.Ask) / pricingItems.Count;
                            var fiveMinuteBid = pricingItems.Sum(m => m.Bid) / pricingItems.Count;
                            childCoin.Price = fiveMinutePrice;
                            childCoin.Ask = fiveMinuteAsk;
                            childCoin.Bid = fiveMinuteBid;
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
        
        public void ListenToCryptoCompareTicker(List<Exchange> exchanges)
        {
            var subParams = BuildCryptoTickerSubscriptionParameters(exchanges);
            var subParamsString = string.Join(",", subParams);
            var subscribeParam = $@"{{ subs: [{subParamsString}]}}";
            _cryptoCompareSocket = IO.Socket("wss://streamer.cryptocompare.com");
            _cryptoCompareSocket.On(Socket.EVENT_CONNECT, () =>
            {
                _cryptoCompareSocket.Emit("SubAdd", JObject.Parse(subscribeParam) );
            });

            _cryptoCompareSocket.On("m", (data) =>
            {
                if (data != null && !string.IsNullOrEmpty(data.ToString()))
                {
                    var dataArray = data.ToString().Split('~');
                    if (dataArray.Length >= 7 && dataArray[0] == "2")
                    {
                        //File.AppendAllLines(@"C:\temp\socketdata.txt", new List<string> { data.ToString() }, Encoding.UTF8);
                        //var previousData = PricingService.Instance.GetCurrentPricingAsStringArray(currentTicker.Market, currentTicker.FromSymbol, currentTicker.ToSymbol);
                        //todo: keep previous record per exchange in ram so we can keep it up to date and use the values that are missing from the update records.

                        var key = $"{dataArray[1]}_{dataArray[2]}_{dataArray[3]}";
                        var previousData = _currentCryptoCompareRows.ContainsKey(key) ? _currentCryptoCompareRows[key] : null;
                        var wrappedData = new SocketDataWrapperDto(dataArray, previousData);
                        var currentTicker = _builderFactory.Create<SocketDataWrapperDto, TickerDto>(wrappedData).Build();
                        //Console.WriteLine(JsonConvert.SerializeObject(currentTicker));
                        var pricingItem = _builderFactory.Create<TickerDto, PricingItem>(currentTicker).Build();
                        PricingService.Instance.UpdatePricingForMinute(currentTicker.Market, currentTicker.FromSymbol, currentTicker.ToSymbol, pricingItem);
                        _currentCryptoCompareRows[key] = wrappedData.PreviousData != null ? wrappedData.PreviousData : wrappedData.Data;
                    }
                }
            });
        }
        
        private List<string> BuildCryptoTickerSubscriptionParameters(List<Exchange> exchanges)
        {
            var results = new List<string>();
            foreach (var exchange in exchanges.Where(m=> new ExchangeApiServiceFactory().Create(m).PricingApiType == PricingApiType.CryptoCompare))
            {
                foreach (var coin in exchange.Coins)
                {
                    foreach (var childCoin in coin.Coins)
                    {
                        results.Add($"'2~{exchange.Name}~{coin.Symbol}~{childCoin.Symbol}'");
                    }
                }
            }
            return results;
        }
    }
}