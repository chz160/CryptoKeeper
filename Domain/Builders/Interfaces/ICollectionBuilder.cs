using System.Collections.Generic;

namespace CryptoKeeper.Domain.Builders.Interfaces
{
    public interface ICollectionBuilder<out TOTYPE> : IBuilder<IEnumerable<TOTYPE>>
    { }
}