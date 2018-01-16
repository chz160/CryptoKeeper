namespace CryptoKeeper.Domain.DataObjects.Dtos.HitBtc
{
    public class SymbolDto
    {
        public string Id { get; set; }
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public string QuantityIncrement { get; set; }
        public string TickSize { get; set; }
        public string TakeLiquidityRate { get; set; }
        public string ProvideLiquidityRate { get; set; }
        public string FeeCurrency { get; set; }
    }
}