namespace CryptoKeeper.Domain.DataObjects.Dtos
{
    public class ApiConfigurationData
    {
        public string Exchange { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Passphrase { get; set; }
        public bool Configured => !string.IsNullOrEmpty(Key);
    }
}
