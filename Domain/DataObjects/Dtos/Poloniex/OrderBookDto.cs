using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos.Poloniex
{
    public class OrderBookDto
    {
        public string FromSymbol { get; set; }
        public string ToSymbol { get; set; }
        public string[][] Asks { get; set; }
        public string[][] Bids { get; set; }
        public string IsFrozen { get; set; }
        public bool IsFrozenBool => IsFrozen == "1" ? true : false;
    }
}