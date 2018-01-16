using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.Services;

namespace CryptoKeeper.TrackBack
{
    class Program
    {
        static void Main(string[] args)
        {
            var exchangeCurrentlyHoldingFunds = ExchangeConstants.Tidex;
            var primaryCoin = SymbolConstants.Btc;
            var initalInvestment = 0.06463713m;
            new SplashScreenService().ShowSplashScreen();
            new TradingService(exchangeCurrentlyHoldingFunds, primaryCoin, initalInvestment).Trade();
        }
    }
}
