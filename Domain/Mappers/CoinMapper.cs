using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers
{
    public class CoinMapper : IUpdateMapper<object, Coin>
    {
        public void Update(dynamic source, Coin target)
        {
            target.Symbol = source.Value.Symbol.Value;
            target.SortOrder = int.Parse(source.Value.SortOrder.Value);
        }
    }
}
