using CryptoKeeper.Domain.DataObjects.Dtos;

namespace CryptoKeeper.Domain.DataObjects.Params.Interfaces
{
    public interface IStoreTrackbackTransferData : IStoreBaseExchangeData, IStoreTotalPercentDiff
    {
        Exchange LowestExchange { get; set; }
        Exchange HighestExchange { get; set; }
        Coin TrackbackToCoin { get; set; }
        Coin TrackbackFromCoin { get; set; }
        decimal TrackbackPercentDiff { get; set; }
    }
}