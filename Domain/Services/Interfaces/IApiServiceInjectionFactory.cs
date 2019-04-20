using CryptoKeeper.Domain.DataObjects.Dtos;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IApiServiceInjectionFactory
    {
        IAmAnApiService Create<T>(Exchange exchange);
        IAmAnApiService Create<T>();
    }
}