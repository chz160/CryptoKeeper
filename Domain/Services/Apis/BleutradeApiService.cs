﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Dtos.Bleutrade;
using CryptoKeeper.Domain.Enums;
using CryptoKeeper.Domain.Services.Apis.PricingMonitors;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Entities.Pricing.Models;

namespace CryptoKeeper.Domain.Services.Apis
{
    public class BleutradeApiService : ApiService
    {
        private readonly Exchange _exchange;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public BleutradeApiService(Exchange exchange, IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IServiceProvider serviceProvider, IBuilderFactory builderFactory)
            : base(configService, cryptoCompareDataService, serviceProvider)
        {
            _exchange = exchange;
            _serviceProvider = serviceProvider;
            _builderFactory = builderFactory;
        }

        public override string Name => ExchangeConstants.Bleutrade;
        public override string PublicUrl => "https://bleutrade.com/api/v2/public";
        public override string PrivateUrl => "";

        public override HMAC GetHMac()
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoder => Encoding.UTF8;
        public override PricingApiType PricingApiType => PricingApiType.Rest;

        public override IAmPricingMonitor MonitorPrices()
        {
            return new BleutradePricingMonitorService(this, _exchange, _builderFactory, _serviceProvider);
        }

        public override List<WithdrawalFee> GetWithdrawalFees()
        {
            var response = Get<ResponseDto<List<CurrencyDto>>>(PublicUrl, "/getcurrencies").result;
            var activeCurrencies = response.Where(m => m.IsActive == "true" && m.MaintenanceMode == "false");
            var withdrawalFees = new BuilderFactory().CreateCollection<CurrencyDto, WithdrawalFee>(activeCurrencies).Build().ToList();
            return withdrawalFees;
        }

        public override void GetProducts(Exchange exchange, List<string> eligibleSymbols)
        {
            var currencies = Get<ResponseDto<List<CurrencyDto>>>(PublicUrl, "/getcurrencies").result.Where(m=>m.IsActive == "true" && m.MaintenanceMode == "false").Select(m=>m.Currency).ToList();
            var markets = Get<ResponseDto<List<MarketDto>>>(PublicUrl, "/getmarkets").result.Where(m => m.IsActive == "true");
            var products = markets.Where(m => 
                eligibleSymbols.Contains(m.MarketCurrency) && 
                currencies.Contains(m.BaseCurrency) && 
                currencies.Contains(m.MarketCurrency)).ToList();
            foreach (var product in products)
            {
                var coin = exchange.Coins.FirstOrDefault(m => m.Symbol == product.MarketCurrency);
                if (coin == null)
                {
                    coin = new Coin { Symbol = product.MarketCurrency };
                    exchange.Coins.Add(coin);
                }
                coin.Coins.Add(new Coin { Symbol = product.BaseCurrency });
            }
        }

        public override decimal GetBalances(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}