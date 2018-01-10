using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.Services;

namespace CryptoKeeper.TrackBack
{
    class Program
    {
        static void Main(string[] args)
        {
            var exchangeCurrentlyHoldingFunds = "Coinbase";
            var primaryCoin = SymbolConstants.Btc;
            var valueCoin = new []{ SymbolConstants.Usd, SymbolConstants.Usdt };
            var initalInvestment = 1000m;
            new TradingService(exchangeCurrentlyHoldingFunds, primaryCoin, valueCoin, initalInvestment).Trade();
        }
    }
}
