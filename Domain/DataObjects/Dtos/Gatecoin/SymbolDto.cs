namespace CryptoKeeper.Domain.DataObjects.Dtos.Gatecoin
{
    public class SymbolDto
    {
        public SymbolDto(string combined, string from, string to)
        {
            Combined = combined;
            From = from;
            To = to;
        }
        public string Combined { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}