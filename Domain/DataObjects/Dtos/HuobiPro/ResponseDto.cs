namespace CryptoKeeper.Domain.DataObjects.Dtos.HuobiPro
{
    public class ResponseDto<T> where T: class
    {
        public string success { get; set; }
        public string ch { get; set; }
        public long ts { get; set; }
        public T data { get; set; }
        public T tick { get; set; }
    }
}
