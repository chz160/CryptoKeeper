namespace CryptoKeeper.Domain.DataObjects.Dtos.Exx
{
    public class TickerDto
    {
        public string fromSymbol { get; set; }
        public string toSymbol { get; set; }
        public string vol { get; set; }
        public string last { get; set; }
        public string buy { get; set; }
        public string sell { get; set; }
        public decimal weekRiseRate { get; set; }
        public decimal riseRate { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public decimal monthRiseRate { get; set; }
    }
}