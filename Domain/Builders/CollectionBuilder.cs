using System.Collections.Generic;
using CryptoKeeper.Domain.Builders.Interfaces;

namespace CryptoKeeper.Domain.Builders
{
    public class CollectionBuilder<FROMTYPE, TOTYPE> : ICollectionBuilder<TOTYPE> where TOTYPE : class, new()
    {
        private readonly IEnumerable<FROMTYPE> _sourcetypes;
        private readonly IBuilderFactory _builderFactory;

        public CollectionBuilder(IEnumerable<FROMTYPE> sourcetypes, IBuilderFactory builderFactory)
        {
            _sourcetypes = sourcetypes;
            _builderFactory = builderFactory;
        }

        public IEnumerable<TOTYPE> Build()
        {
            var toList = new List<TOTYPE>();

            if (_sourcetypes != null)
            {
                foreach (var fromtype in _sourcetypes)
                {
                    toList.Add(_builderFactory.Create<FROMTYPE, TOTYPE>(fromtype).Build());
                }
            }
            return toList;
        }
    }
}