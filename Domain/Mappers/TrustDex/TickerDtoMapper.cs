using System.Collections.Generic;
using System.Linq;
using CryptoKeeper.Domain.DataObjects.Dtos.TrustDex;
using CryptoKeeper.Domain.Mappers.Interfaces;
using Newtonsoft.Json.Linq;

namespace CryptoKeeper.Domain.Mappers.TrustDex
{
    public class TickerDtoMapper : IUpdateMapper<JToken, TickerDto>
    {
        public void Update(JToken sourceType, TickerDto updateType)
        {
            var token = sourceType.Cast<KeyValuePair<string, JToken>>().ToList().First().Value;
            updateType.id = token["id"].ToString();
            updateType.coin = token["coin"].ToString();
            updateType.last = token["last"].ToString();
            updateType.highestBid = token["highestBid"].ToString();
            updateType.lowestAsk = token["lowestAsk"].ToString();
            updateType.percentChange = token["percentChange"].ToString();
            updateType.baseVolume = token["baseVolume"].ToString();
            updateType.quoteVolume = token["quoteVolume"].ToString();
            updateType.isFrozen = token["isFrozen"].ToString();
            updateType.high24hr = token["high24hr"].ToString();
            updateType.low24hr = token["low24hr"].ToString();
        }
    }
}
