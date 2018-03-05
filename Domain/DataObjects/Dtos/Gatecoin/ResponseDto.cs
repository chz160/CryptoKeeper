namespace CryptoKeeper.Domain.DataObjects.Dtos.Gatecoin
{
    public class ResponseDto<T> where T : class 
    {
        public T tickers { get; set; }
    }
}