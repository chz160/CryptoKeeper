using CryptoKeeper.Domain.DataObjects.Dtos.Exx;
using CryptoKeeper.Domain.Mappers.Interfaces;
using Newtonsoft.Json.Linq;

namespace CryptoKeeper.Domain.Mappers.Exx
{
    public class TickerDtoMapper : IUpdateMapper<JToken, TickerDto>
    {
        public void Update(JToken sourceType, TickerDto updateType)
        {
            var jProp = (JProperty)sourceType;
            var token = ((JProperty)sourceType).Value;
            updateType.fromSymbol = jProp.Name.Split("_")[0].ToUpper();
            updateType.toSymbol = jProp.Name.Split("_")[1].ToUpper();
            updateType.vol = (string)token["vol"];
            updateType.last = (string)token["last"];
            updateType.buy = (string)token["buy"];
            updateType.sell = (string)token["sell"];
            updateType.weekRiseRate = (decimal)token["weekRiseRate"];
            updateType.riseRate = (decimal)token["riseRate"];
            updateType.high = (string)token["high"];
            updateType.low = (string)token["low"];
            updateType.monthRiseRate = (decimal)token["monthRiseRate"];
        }
    }
}