namespace CryptoKeeper.Domain.DataObjects.Dtos.CexIo
{
    public class PairDto
    {
        public string symbol1 { get; set; }
        public string symbol2 { get; set; }
        public decimal minLotSize { get; set; }
        public decimal minLotSizeS2 { get; set; }
        public decimal? maxLotSize { get; set; }
        public string minPrice { get; set; }
        public string maxPrice { get; set; }
    }
}