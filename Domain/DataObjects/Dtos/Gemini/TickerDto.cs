using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos.Gemini
{
    public class TickerDto
    {
        public string bid { get; set; }
        public string ask { get; set; }
        public Dictionary<string, string> volume { get; set; }
    }
}
