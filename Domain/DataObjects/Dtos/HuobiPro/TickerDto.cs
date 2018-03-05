namespace CryptoKeeper.Domain.DataObjects.Dtos.HuobiPro
{
    public class TickerDto
    {
        public long id { get; set; }
        public decimal amount { get; set; }
        public decimal open { get; set; }
        public decimal close { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public int count { get; set; }
        public long version { get; set; }
        public decimal[] ask { get; set; }
        public decimal[] bid { get; set; }
        public decimal vol { get; set; }
    }
}