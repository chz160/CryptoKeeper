namespace CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare
{
    public class SocketDataWrapperDto
    {
        public SocketDataWrapperDto(string[] data, string[] previousData)
        {
            Data = data;
            PreviousData = previousData;
        }
        public string[] Data { get; set; }
        public string[] PreviousData { get; set; }
    }
}
