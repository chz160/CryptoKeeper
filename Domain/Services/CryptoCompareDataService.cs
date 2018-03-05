using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.Services.Interfaces;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Services
{
    public class CryptoCompareDataService : ICryptoCompareDataService
    {
        private readonly IConfigService _configService;
        private const string BaseUrl = "https://min-api.cryptocompare.com/data";

        public CryptoCompareDataService() : this(new ConfigService())
        { }

        public CryptoCompareDataService(IConfigService configService)
        {
            _configService = configService;
        }

        public List<Exchange> GetAllExchanges()
        {
            var exchanges = new List<Exchange>();
            var exchangeData = GetData<Dictionary<string, Dictionary<string, List<string>>>>($"{BaseUrl}/all/exchanges");
            foreach (var exchange in exchangeData)
            {
                var exchangeDto = new Exchange { Name = exchange.Key };
                foreach (var coin in exchange.Value)
                {
                    var coinDto = new Coin { Symbol = coin.Key };
                    foreach (var convertionCoin in coin.Value)
                    {
                        var symbol = convertionCoin;
                        coinDto.Coins.Add(new Coin { Symbol = symbol });
                    }
                    if (coinDto.Coins.Any())
                    {
                        exchangeDto.Coins.Add(coinDto);
                    }
                }
                exchanges.Add(exchangeDto);
            }
            return exchanges;
        }

        public List<Exchange> GetTopExchangesForPair(string fsym, string tsym)
        {
            Console.Write($"Getting top exchanges for {fsym} -> {tsym}...");
            var url = $"{BaseUrl}/top/exchanges?fsym={fsym}&tsym={tsym}&limit=20";
            var exchanges = new List<Exchange>();
            dynamic exchangeData = GetData<dynamic>(url);
            foreach (dynamic exchange in exchangeData["Data"])
            {
                var exchangeDto = new Exchange { Name = exchange.exchange.Value };
                //Console.WriteLine($"\t{exchangeDto.Name}");
                foreach (dynamic coin in new[] { fsym, tsym }.Where(m => m != SymbolConstants.Usd))
                {
                    var coinDto = new Coin { Symbol = coin };
                    exchangeDto.Coins.Add(coinDto);
                }
                exchanges.Add(exchangeDto);
            }
            Console.WriteLine("Done.");
            //return exchanges.Where(m => !_exchangeExclusions.Contains(m.Name)).ToList();
            var configuredApis = _configService.GetApiConfigurations();
            if (configuredApis == null || !configuredApis.Any()) throw new Exception("There must be at least two configured APIs in the api.json file.");
            return exchanges.Where(m => configuredApis.Where(n=>n.Configured).Select(n=>n.Exchange).ToList().Contains(m.Name)).ToList();
        }

        public void GetExchangeCoins(string primaryCoin, string valueCoin, List<Exchange> exchanges, List<string> symbols = null)
        {
            var completeExchanges = GetAllExchanges();
            foreach (var exchange in exchanges)
            {
                var coins = completeExchanges.First(m => m.Name == exchange.Name).Coins;
                if (symbols?.Any() ?? false)
                {
                    coins = coins.Where(m => symbols.Contains(m.Symbol)).ToList();
                }
                coins = coins.Where(m => !SymbolConstants.FiatCurrency.Contains(m.Symbol)).ToList();
                coins.ForEach(m =>
                {
                    var coinsToRemove = m.Coins.Where(n => SymbolConstants.FiatCurrency.Contains(n.Symbol)).ToList();
                    coinsToRemove.ForEach(n => m.Coins.Remove(n));
                });
                exchange.Coins = coins.Where(m=>m.Coins.Any()).ToList();
            }
        }

        public List<string> GetTopVolumeSymbols(string tsym)
        {
            var url = $"{BaseUrl}/top/volumes?tsym={tsym}&limit=25";
            var symbols = new List<string> { tsym };
            dynamic symbolData = GetData<dynamic>(url);
            foreach (dynamic symbol in symbolData["Data"])
            {
                symbols.Add(symbol.SYMBOL.Value);
            }
            return symbols;
        }

        public HistoMinuteList GetCoinHistoryForExchange(string exchange, string fromSymbol, string toSymbol, long timestamp)
        {
            var url = $"{BaseUrl}/histominute?fsym={fromSymbol}&tsym={toSymbol}&toTs={timestamp}&limit=1440&e={exchange}";
            return new HistoMinuteList(GetData<HistoMinuteDto>(url));
        }

        public HistoMinuteList GetCoinForExchange(string exchange, string fromSymbol, string toSymbol)
        {
            var url = $"{BaseUrl}/histominute?fsym={fromSymbol}&tsym={toSymbol}&limit=5&e={exchange}";
            return new HistoMinuteList(GetData<HistoMinuteDto>(url));
        }

        int retry = 0;
        public T GetData<T>(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    using (StreamReader reader = new StreamReader(client.OpenRead(url)))
                    {
                        string json = reader.ReadToEnd();
                        retry = 0;
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            catch (WebException ex)
            {
                if (retry < 5)
                {
                    //Thread.Sleep(5000);
                    Task.Delay(5000).Wait();
                    retry++;
                    return GetData<T>(url);
                }
                throw;
            }

        }
    }
}
