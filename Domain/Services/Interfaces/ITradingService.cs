namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface ITradingService
    {
        void StartProcess(string exchangeCurrentlyHoldingFunds, string primaryCoin, decimal investment);
    }
}