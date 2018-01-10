using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos.Yobit
{
    public class ResponseDto
    {
        public long Server_Time { get; set; }
        public Dictionary<string, InfoDto> Pairs { get; set; }
    }
}
