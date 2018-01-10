using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Mappers.Factories;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Builders
{
    public class CreationBuilder<FROMTYPE, TOTYPE> : IBuilder<TOTYPE> where TOTYPE : class, new()
    {
        private readonly FROMTYPE _source;
        private readonly IMapperFactory _mapperFactory;

        public CreationBuilder(FROMTYPE source) : this(source, new MapperFactory())
        {
            _source = source;
        }

        public CreationBuilder(FROMTYPE source, IMapperFactory mapperFactory)
        {
            _source = source;
            _mapperFactory = mapperFactory;
        }

        public TOTYPE Build()
        {
            var target = new TOTYPE();
            _mapperFactory.CreateUpdate<FROMTYPE, TOTYPE>().Update(_source, target);
            return target;
        }
    }
}