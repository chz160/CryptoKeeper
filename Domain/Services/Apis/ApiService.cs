using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Domain.Utilities;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Services.Apis
{
    public abstract class ApiService : IAmAnApiService
    {
        protected readonly IConfigService _configService;
        private readonly ICryptoCompareDataService _cryptoCompareDataService;
        protected readonly ApiConfigurationData _configurationData;

        public ApiService() : this(new ConfigService(), new CryptoCompareDataService())
        { }

        public ApiService(IConfigService configService, ICryptoCompareDataService cryptoCompareDataService)
        {
            _configService = configService;
            _cryptoCompareDataService = cryptoCompareDataService;
            _configurationData = configService.GetApiConfigurationForExchange(Name);
        }
        public abstract string Name { get; }
        public abstract string PublicUrl { get; }
        public abstract string PrivateUrl { get; }
        public virtual string ContentType => "application/json";
        public virtual string Key => _configurationData?.Key;
        public virtual string Secret => _configurationData?.Secret;
        public virtual string Passphrase => _configurationData?.Passphrase;
        public virtual bool MustBeSigned => true;
        public virtual bool RequiresNonce => true;
        public virtual bool PlaceParametersInUrl => false;
        public abstract HMAC GetHMac();
        public virtual bool SignAsHex => false;
        public virtual Encoding Encoder => Encoding.ASCII;
        public abstract PricingApiType PricingApiType { get; }
        public abstract decimal MakerFee { get; }
        public abstract decimal TakerFee { get; }
        public virtual List<WithdrawalFee> WithdrawalFees => PricingService.Instance.GetWithdrawalFeesForExchange(this);
        public abstract List<WithdrawalFee> GetWithdrawalFees();

        protected virtual string SignString(string message)
        {
            if (message != null)
            {
                var byteArray = Encoder.GetBytes(message);
                using (var hmac = GetHMac())
                {
                    var hashArray = hmac.ComputeHash(byteArray);
                    if (SignAsHex)
                    {
                        return hashArray.Aggregate("", (s, e) => s + string.Format("{0:x2}", e), s => s);
                    }
                    return Convert.ToBase64String(hashArray);
                }
            }
            return string.Empty;
        }

        protected abstract void BuildHeaders(HttpWebRequest request, string baseUrl, string relativeUrl, string body);

        public T Get<T>(string baseUrl, string relativeUrl, Dictionary<string, object> dictionary)
        {
            if (RequiresNonce)
            {
                dictionary.Add("nonce", Helpers.GetCurrentHttpPostNonce());
            }
            return Get<T>(baseUrl, relativeUrl, dictionary.ToHttpPostString());
        }

        public T Get<T>(string baseUrl, string relativeUrl, string body = null)
        {
            T result;
            if (PlaceParametersInUrl)
            {
                relativeUrl = $"{relativeUrl}?{body}";
            }
            var request = CreateHttpWebRequest("GET", baseUrl, relativeUrl);
            BuildHeaders(request, baseUrl, relativeUrl, body);
            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                var responseText = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(responseText);
            }
            return result;
        }

        public T Post<T>(string baseUrl, string relativeUrl, Dictionary<string, object> dictionary)
        {
            if (RequiresNonce)
            {
                dictionary.Add("nonce", Helpers.GetCurrentHttpPostNonce());
            }
            return Post<T>(baseUrl, relativeUrl, dictionary.ToHttpPostString());
        }

        public T Post<T>(string baseUrl, string relativeUrl, string body)
        {
            dynamic result = null;

            var request = CreateHttpWebRequest("POST", baseUrl, relativeUrl);

            //TODO: when the body needs to be json how do we add nonce

            //if (RequiresNonce && ContentType == "application/x-www-form-urlencoded")
            //{
            //    var delimeter = string.IsNullOrEmpty(body) ? "" : "&";
            //    var nonce = Helpers.GetCurrentHttpPostNonce();
            //    body = $"{body}{delimeter}nonce={nonce}";
            //}

            var postBytes = Encoder.GetBytes(body);
            request.ContentLength = postBytes.Length;

            BuildHeaders(request, baseUrl, relativeUrl, body);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(postBytes, 0, postBytes.Length);
            }
            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                var responseText = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(responseText);
            }
            return result;
        }

        private HttpWebRequest CreateHttpWebRequest(string method, string baseUrl, string relativeUrl)
        {
            var url = baseUrl + relativeUrl;
            var request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.UserAgent = "CryptoKeeper";
            request.Timeout = Timeout.Infinite;
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentType = ContentType;
            return request;
        }

        public abstract IAmPricingMonitor MonitorPrices();
        
        public virtual long GetServerTime()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public abstract decimal GetBalances(string symbol);

        public virtual void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            _cryptoCompareDataService.GetExchangeCoins(null, null, new List<Exchange> { exchange }, eligibleSymbols);
        }
    }
}