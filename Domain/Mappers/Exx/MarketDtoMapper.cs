using CryptoKeeper.Domain.DataObjects.Dtos.Exx;
using CryptoKeeper.Domain.Mappers.Interfaces;
using Newtonsoft.Json.Linq;

namespace CryptoKeeper.Domain.Mappers.Exx
{
    public class MarketDtoMapper : IUpdateMapper<JToken, MarketDto>
    {
        public void Update(JToken sourceType, MarketDto updateType)
        {
            var jProp = (JProperty) sourceType;
            var token = ((JProperty)sourceType).Value;
            updateType.fromSymbol = jProp.Name.Split("_")[0].ToUpper();
            updateType.toSymbol = jProp.Name.Split("_")[1].ToUpper();
            updateType.minAmount = token["minAmount"].ToString();
            updateType.amountScale = (int)token["amountScale"];
            updateType.priceScale = (int)token["priceScale"];
            updateType.maxLevels = (int)token["maxLevels"];
            updateType.isOpen = (bool)token["isOpen"];
        }
    }
}
