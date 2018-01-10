using CryptoKeeper.Domain.DataObjects.Dtos;

namespace CryptoKeeper.Domain.DataObjects.Params.Interfaces
{
    public interface IStoreDirectTransferData : IStoreBaseExchangeData, IStoreTotalPercentDiff
    {
        Exchange LowestExchange { get; set; }
        Exchange HighestExchange { get; set; }
        decimal PrimaryPercentDiff { get; set; }
    }
}