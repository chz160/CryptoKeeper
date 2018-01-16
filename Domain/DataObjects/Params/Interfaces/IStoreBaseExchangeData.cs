using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;

namespace CryptoKeeper.Domain.DataObjects.Params.Interfaces
{
    public interface IStoreBaseExchangeData : IStoreExchangeCurrentlyHoldingFunds
    {
        string PrimaryCoin { get; }
        
        List<string> EligibleSymbols { get; set; }
        List<Exchange> Exchanges { get; set; }
    }
}