namespace CryptoKeeper.Domain.DataObjects.Dtos.Coinbase
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string Base_Currency { get; set; }
        public string Quote_Currency { get; set; }
        public string Base_Min_Size { get; set; }
        public string Base_Max_Size { get; set; }
        public string Quote_Increment { get; set; }
        public string Display_Name { get; set; }
        public string Status { get; set; }
        public bool Margin_Enabled { get; set; }
        public string Status_Message { get; set; }
    }
}