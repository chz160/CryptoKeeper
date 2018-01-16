using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos.Binance
{
    public class ResponseDto
    {
        public string timezone { get; set; }
        public long serverTime { get; set; }
        public List<SymbolDto> symbols { get; set; }
    }
}
