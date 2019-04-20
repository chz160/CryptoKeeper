namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface ITradingServiceInjectionFactory
    {
        ITradingService Create(string exchangeCurrentlyHoldingFunds, string primaryCoin, decimal investment);
    }
}