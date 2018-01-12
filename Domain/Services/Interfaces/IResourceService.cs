namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IResourceService
    {
        byte[] ExtractResource(string filename);
    }
}