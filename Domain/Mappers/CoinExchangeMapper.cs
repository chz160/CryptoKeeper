using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers
{
    public class CoinExchangeMapper : IUpdateMapper<object, CoinExchange>
    {
        public void Update(dynamic source, CoinExchange target)
        {
            target.Symbol = source.toSymbol.Value;
            target.Exchange = source.exchange.Value;
        }
    }
}