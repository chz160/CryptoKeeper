using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface ICryptoCompareDataService
    {
        List<Exchange> GetAllExchanges();
        List<Exchange> GetTopExchangesForPair(string fsym, string tsym);
        void GetExchangeCoins(string primaryCoin, string valueCoin, List<Exchange> exchanges, List<string> symbols = null);
        List<string> GetTopVolumeSymbols(string tsym);
        HistoMinuteList GetCoinHistoryForExchange(string exchange, string fromSymbol, string toSymbol, long timestamp);
        HistoMinuteList GetCoinForExchange(string exchange, string fromSymbol, string toSymbol);
        T GetData<T>(string url);
    }
}