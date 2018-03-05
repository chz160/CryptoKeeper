using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos
{
    public class Coin
    {
        public Coin()
        {
            Coins = new List<Coin>();
            CoinExchanges = new List<CoinExchange>();
        }
        public string Symbol { get; set; }
        public int SortOrder { get; set; }
        public decimal Price { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public List<Coin> Coins { get; set; }
        public List<CoinExchange> CoinExchanges { get; set; }
    }
}