namespace CryptoKeeper.Domain.DataObjects.Dtos.Bleutrade
{
    public class ResponseDto<T> where T: class
    {
        public string success { get; set; }
        public string message { get; set; }
        public T result { get; set; }
    }
}
