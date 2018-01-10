namespace CryptoKeeper.Domain.DataObjects.Dtos.Poloniex
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TxFee { get; set; }
        public int MinConf { get; set; }
        public string DepositAddress { get; set; }
        public bool Disabled { get; set; }
        public bool Delisted { get; set; }
        public bool Frozen { get; set; }
    }
}
