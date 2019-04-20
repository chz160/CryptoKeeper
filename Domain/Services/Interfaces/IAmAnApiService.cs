using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Entities.Pricing.Models;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IAmAnApiService
    {
        string Name { get; }
        string PublicUrl { get; }
        string PrivateUrl { get; }
        string ContentType { get; }
        string Key { get; }
        string Secret { get; }
        string Passphrase { get; }
        bool MustBeSigned { get; }
        bool RequiresNonce { get; }
        bool PlaceParametersInUrl { get; }
        HMAC GetHMac();
        bool SignAsHex { get; }
        Encoding Encoder { get; }
        PricingApiType PricingApiType { get; }
        IAmPricingMonitor MonitorPrices();
        decimal TakerFee { get; }
        decimal MakerFee { get; }
        List<WithdrawalFee> WithdrawalFees { get; }
        List<WithdrawalFee> GetWithdrawalFees();
        T Get<T>(string baseUrl, string relativeUrl, Dictionary<string, object> dictionary);
        T Get<T>(string baseUrl, string relativeUrl, string body = null);
        T Post<T>(string baseUrl, string relativeUrl, Dictionary<string, object> dictionary);
        T Post<T>(string baseUrl, string relativeUrl, string body);

        long GetServerTime();
        decimal GetBalances(string symbol);
        void GetProducts(Exchange exchange, List<string> eligibleSymbols);
    }
}