using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers;

namespace CryptoKeeper.Domain.Builders
{
    public class CoinBuilder : IBuilder<Coin>
    {
        private readonly dynamic _source;

        public CoinBuilder(dynamic source)
        {
            _source = source;
        }
        
        public Coin Build()
        {
            var target = new Coin();
            new CoinMapper().Update(_source, target);
            return target;
        }
    }
}
