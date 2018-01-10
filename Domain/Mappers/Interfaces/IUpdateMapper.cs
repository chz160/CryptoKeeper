namespace CryptoKeeper.Domain.Mappers.Interfaces
{
    public interface IUpdateMapper<in SOURCETYPE, in TARGETTYPE>
    {
        void Update(SOURCETYPE sourceType, TARGETTYPE updateType);
    }

    public interface IMapper<in SOURCETYPE, out TARGETTYPE>
    {
        TARGETTYPE MapFrom(SOURCETYPE source);
    }
}
