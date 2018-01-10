namespace CryptoKeeper.Domain.Builders.Interfaces
{
    public interface IBuilder<out T>
    {
        T Build();
    }
}
