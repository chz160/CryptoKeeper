using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers.Factories
{
    public class CreationMapper<SOURCETYPE, TARGETTYPE> : IMapper<SOURCETYPE, TARGETTYPE> where TARGETTYPE : class, new()
    {
        private readonly IMapperFactory _mapperFactory;

        public CreationMapper() : this (new MapperFactory())
        { }

        internal CreationMapper(IMapperFactory mapperFactory)
        {
            _mapperFactory = mapperFactory;
        }

        public TARGETTYPE MapFrom(SOURCETYPE source)
        {
            var newTarget = Create();
            _mapperFactory.CreateUpdate<SOURCETYPE, TARGETTYPE>().Update(source, newTarget);
            return newTarget;
        }

        protected virtual TARGETTYPE Create()
        {
            return new TARGETTYPE();
        }
    }
}