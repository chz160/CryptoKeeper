using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Params.Interfaces;

namespace CryptoKeeper.Domain.DataObjects.Params
{
    public abstract class ExchangeParam : IStoreBaseExchangeData
    {
        public ExchangeParam(string primaryCoin, string[] valueCoin, string exchangeCurrentlyHoldingFunds, List<string> eligibleSymbols, List<Exchange> exchanges)
        {
            PrimaryCoin = primaryCoin;
            ValueCoin = valueCoin;
            ExchangeCurrentlyHoldingFunds = exchangeCurrentlyHoldingFunds;
            EligibleSymbols = eligibleSymbols;
            Exchanges = exchanges;
        }

        public string PrimaryCoin { get; set; }
        public string[] ValueCoin { get; set; }
        public string ExchangeCurrentlyHoldingFunds { get; set; }
        public List<string> EligibleSymbols { get; set; }
        public List<Exchange> Exchanges { get; set; }
    }
}