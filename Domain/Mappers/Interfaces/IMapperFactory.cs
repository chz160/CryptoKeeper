namespace CryptoKeeper.Domain.Mappers.Interfaces
{
    public interface IMapperFactory
    {
        IUpdateMapper<SOURCETYPE, TOTYPE> CreateUpdate<SOURCETYPE, TOTYPE>() where TOTYPE : class, new();
    }
}
