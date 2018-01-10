using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos
{
    public class Exchange
    {
        public Exchange()
        {
            Coins = new List<Coin>();
        }
        public string Name { get; set; }

        public List<Coin> Coins { get; set; }
    }
}