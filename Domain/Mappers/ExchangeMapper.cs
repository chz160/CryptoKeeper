using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers
{
    public class ExchangeMapper : IUpdateMapper<ApiConfigurationData, Exchange>
    {
        public void Update(ApiConfigurationData sourceType, Exchange updateType)
        {
            updateType.Name = sourceType.Exchange;
        }
    }
}