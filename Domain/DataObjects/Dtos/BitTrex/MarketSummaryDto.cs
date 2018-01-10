namespace CryptoKeeper.Domain.DataObjects.Dtos.BitTrex
{
    public class MarketSummaryDto
    {
        public string MarketName { get; set; }
        public string BaseCurrency => MarketName.Split("-")[0];
        public string MarketCurrency => MarketName.Split("-")[1];
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }
        public decimal Last { get; set; }
        public decimal BaseVolume { get; set; }
        public string TimeStamp { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public int OpenBuyOrders { get; set; }
        public int OpenSellOrders { get; set; }
        public decimal PrevDay { get; set; }
        public string Created { get; set; }
    }
}