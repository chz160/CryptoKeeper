using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.DataObjects.Params;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Entities.Pricing.Models;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IPricingService
    {
        HistoMinuteList GetPriceHistory(string exchange, string fromSymbol, string toSymbol, long timestamp);
        HistoMinuteItem GetCurrentPrice(string exchange, string fromSymbol, string toSymbol);
        string[] GetCurrentPricingAsStringArray(string exchange, string fromSymbol, string toSymbol);
        List<PricingItem> GetPricingForLast24Hours(string exchange, string fromSymbol, string toSymbol, long timestamp);
        void UpdatePricingForMinute(string exchange, string fromSymbol, string toSymbol, PricingItem item);
        void UpdatePricingForMinute(string exchange, string fromSymbol, string toSymbol, List<PricingItem> items);
        decimal CalculateTrackbackWeight(string fromExchange, string toExchange, string fromSymbol, string toSymbol, decimal trackBackPercentDiff);
        void StartPricingDataThreads(ExchangePairParam exchangePairParam);
        void PrintPrimingReport(List<Exchange> exchanges);
        void UpdateExchangeCoinPrices(List<Exchange> exchanges);
        void UpdateExchangeCoinPrices(List<Exchange> allExchanges, List<string> exchangeNames, List<string> symbols);
        void PrimeWithdrawalFeeList(List<Exchange> exchanges);
        List<WithdrawalFee> GetWithdrawalFeesForExchange(IAmAnApiService api);
        decimal GetWithdrawalFeesForExchangeAndSymbol(IAmAnApiService api, string symbol);
        decimal GetWithdrawalFeesForExchangeAndSymbol(Exchange exchange, string symbol);
        decimal GetAverageFeeForCoinAcrossExchanges(string symbol);
        decimal GetTakerFeeForExchange(Exchange exchange);
        void ListenToCryptoCompareTicker(List<Exchange> exchanges);
    }
}