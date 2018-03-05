using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos.CexIo
{
    public class MarketDto
    {
        public List<PairDto> pairs { get; set; }
    }
}
