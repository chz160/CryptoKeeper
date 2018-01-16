namespace CryptoKeeper.Domain.DataObjects.Dtos.Bleutrade
{
    public class CurrencyDto {
        public string Currency { get; set; }
        public string CurrencyLong { get; set; }
        public string MinConfirmation { get; set; }
        public string TxFee { get; set; }
        public string CoinType { get; set; }
        public string IsActive { get; set; }
        public string MaintenanceMode { get; set; }
    }
}