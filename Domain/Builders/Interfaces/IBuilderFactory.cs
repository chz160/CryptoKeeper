using System.Collections.Generic;

namespace CryptoKeeper.Domain.Builders.Interfaces
{
    public interface IBuilderFactory
    {
        IBuilder<TOTYPE> Create<FROMTYPE, TOTYPE>(FROMTYPE fromtype) where TOTYPE : class, new();
        IBuilder<IEnumerable<TOTYPE>> CreateCollection<FROMTYPE, TOTYPE>(IEnumerable<FROMTYPE> fromtype) where TOTYPE : class, new();
    }
}