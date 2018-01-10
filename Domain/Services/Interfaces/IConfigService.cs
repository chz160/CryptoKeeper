using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IConfigService
    {
        List<ApiConfigurationData> GetApiConfigurations();
        ApiConfigurationData GetApiConfigurationForExchange(string exchange);
        List<Exchange> GetConfiguredExchanges();
    }
}