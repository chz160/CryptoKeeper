namespace CryptoKeeper.Domain.DataObjects.Dtos.Yobit
{
    public class InfoDto
    {
        public int Decimal_Places { get; set; }
        public decimal Min_Price { get; set; }
        public decimal Max_Price { get; set; }
        public decimal Min_Amount { get; set; }
        public decimal Min_Total { get; set; }
        public bool Hidden { get; set; }
        public decimal Fee { get; set; }
        public decimal Fee_Buyer { get; set; }
        public decimal Fee_Seller { get; set; }
    }
}