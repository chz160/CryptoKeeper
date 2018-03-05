namespace CryptoKeeper.Domain.DataObjects.Dtos.BXinth
{
    public class PairingDto
    {
        public string pairing_id { get; set; }
        public string primary_currency { get; set; }
        public string secondary_currency { get; set; }
        public string primary_min { get; set; }
        public string secondary_min { get; set; }
        public bool active { get; set; }
    }

    public class MarketDto
    {
        public string pairing_id { get; set; }
        public string primary_currency { get; set; }
        public string secondary_currency { get; set; }
        public decimal change { get; set; }
        public decimal last_price { get; set; }
        public decimal volume_24hours { get; set; }
        public OrderBookDto orderbook { get; set; }

        public class OrderBookDto
        {
            public BidDto bids { get; set; }
            public AskDto asks { get; set; }

            public class BidDto
            {
                public int total { get; set; }
                public decimal volume { get; set; }
                public decimal highbid { get; set; }
            }

            public class AskDto
            {
                public int total { get; set; }
                public decimal volume { get; set; }
                public decimal highbid { get; set; }
            }
        }
    }
}
