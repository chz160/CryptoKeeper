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
        public virtual decimal MakerFee => 0.0025m;
        public virtual decimal TakerFee => 0.0025m;
        public virtual List<WithdrawalFee> WithdrawalFees => PricingService.Instance.GetWithdrawalFeesForExchange(this);

        public virtual List<WithdrawalFee> GetWithdrawalFees()
        {
            return new List<WithdrawalFee>{
                new WithdrawalFee {Symbol = "BTC", Fee = 0.001m},
                new WithdrawalFee {Symbol = "LTC", Fee = 0.003m},
                new WithdrawalFee {Symbol = "ETH", Fee = 0.00958m},
                new WithdrawalFee {Symbol = "BCH", Fee = 0.0018m},
                new WithdrawalFee {Symbol = "BTG", Fee = 0.0005m},
                new WithdrawalFee {Symbol = "ETC", Fee = 0.002m},
                new WithdrawalFee {Symbol = "PPC", Fee = 0.02m},
                new WithdrawalFee {Symbol = "DASH", Fee = 0.03m},
                new WithdrawalFee {Symbol = "ZEC", Fee = 0.0001m},
                new WithdrawalFee {Symbol = "XRP", Fee = 0.509m},
                new WithdrawalFee {Symbol = "EOS", Fee = 1.5m},
                new WithdrawalFee {Symbol = "TRX", Fee = 270m},
                new WithdrawalFee {Symbol = "WTC", Fee = 0.75m},
                new WithdrawalFee {Symbol = "VEN", Fee = 14m},
                new WithdrawalFee {Symbol = "RLC", Fee = 1.21m},
                new WithdrawalFee {Symbol = "GTO", Fee = 1.21m},
                new WithdrawalFee {Symbol = "QTUM", Fee = 0.04m},
                new WithdrawalFee {Symbol = "XVG", Fee = 0.002m},
                new WithdrawalFee {Symbol = "NMC", Fee = 0.002m},
                new WithdrawalFee {Symbol = "VIA", Fee = 0.002m},
                new WithdrawalFee {Symbol = "DGB", Fee = 0.002m},
                new WithdrawalFee {Symbol = "EOS", Fee = 10m},
                new WithdrawalFee {Symbol = "BNB", Fee = 0.002m},
                new WithdrawalFee {Symbol = "WASH", Fee = 0.01m},
                new WithdrawalFee {Symbol = "SC", Fee = 0.1m},
                new WithdrawalFee {Symbol = "XMR", Fee = 0.04m},
                new WithdrawalFee {Symbol = "ADA", Fee = 0.2m},
                new WithdrawalFee {Symbol = "XLM", Fee = 0.2m},
                new WithdrawalFee {Symbol = "VIBE", Fee = 145m},
                new WithdrawalFee {Symbol = "DOGE", Fee = 2m},
                new WithdrawalFee {Symbol = "STRAT", Fee = 2m}
            };
        }

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