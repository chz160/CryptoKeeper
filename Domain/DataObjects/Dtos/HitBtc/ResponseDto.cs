namespace CryptoKeeper.Domain.DataObjects.Dtos.HitBtc
{
    public class ResponseDto<T> where T : class
    {
        public string JsonRpc { get; set; }
        public string Method { get; set; }
        public T Params { get; set; }
    }
}