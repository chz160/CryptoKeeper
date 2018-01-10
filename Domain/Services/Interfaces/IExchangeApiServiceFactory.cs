using CryptoKeeper.Domain.DataObjects.Dtos;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IExchangeApiServiceFactory
    {
        IAmAnApiService Create(Exchange exchange);
    }
}