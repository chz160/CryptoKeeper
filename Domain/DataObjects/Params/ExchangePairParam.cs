using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Params.Interfaces;

namespace CryptoKeeper.Domain.DataObjects.Params
{
    public class ExchangePairParam : ExchangeParam, IStoreDirectTransferData, IStoreTrackbackTransferData
    {
        //for serialization only 
        public ExchangePairParam() : base(null, null, null, null)
        { }
        public ExchangePairParam(string primaryCoin, string exchangeCurrentlyHoldingFunds, List<string> eligibleSymbols, List<Exchange> exchanges) 
            : base(primaryCoin, exchangeCurrentlyHoldingFunds, eligibleSymbols, exchanges)
        { }

        public Exchange LowestExchange { get; set; }
        public Exchange HighestExchange { get; set; }
        public decimal PrimaryPercentDiff { get; set; }
        public Coin TrackbackToCoin { get; set; }
        public Coin TrackbackFromCoin { get; set; }
        public decimal TrackbackPercentDiff { get; set; }
        public decimal TotalPercentDiff { get; set; }
    }
}
