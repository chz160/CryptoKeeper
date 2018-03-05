namespace CryptoKeeper.Domain.DataObjects.Dtos.Exmo
{
    public class TickerDto
    {
        public string from_symbol { get; set; }
        public string to_symbol { get; set; }
        public string buy_price { get; set; }
        public string sell_price { get; set; }
        public string last_trade { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string avg { get; set; }
        public string vol { get; set; }
        public string vol_curr { get; set; }
        public long updated { get; set; }
    }
}
