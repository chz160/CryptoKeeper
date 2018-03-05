namespace CryptoKeeper.Domain.DataObjects.Dtos.CexIo
{
    public class ResponseDto<T> where T : class
    {
        public string e { get; set; }
        public string ok { get; set; }
        public T data { get; set; }
    }
}