namespace CryptoKeeper.Domain.DataObjects.Dtos.BitTrex
{
    public class ResponseDto<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}