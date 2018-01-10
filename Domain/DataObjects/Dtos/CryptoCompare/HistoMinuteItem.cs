namespace CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare
{
    public class HistoMinuteItem
    {
        public long time { get; set; }
        public decimal? close { get; set; }
        public decimal? high { get; set; }
        public decimal? low { get; set; }
        public decimal? open { get; set; }
        public decimal? volumefrom { get; set; }
        public decimal? volumeto { get; set; }
    }
}