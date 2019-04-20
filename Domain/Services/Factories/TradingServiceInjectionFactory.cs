//using CryptoKeeper.Domain.Services.Interfaces;

//namespace CryptoKeeper.Domain.Services.Factories
//{
//    public class TradingServiceInjectionFactory : ITradingServiceInjectionFactory
//    {
//        private readonly IMathService _mathService;
//        private readonly IExchangeApiServiceFactory _apiServiceFactory;
//        private readonly ICryptoCompareDataService _cryptoCompareDataService;
//        private readonly IConfigService _configService;
//        private readonly IEmailService _emailService;
//        private readonly IPricingService _pricingService;

//        public TradingServiceInjectionFactory(IMathService mathService, IExchangeApiServiceFactory apiServiceFactory, ICryptoCompareDataService cryptoCompareDataService, IConfigService configService, IEmailService emailService, IPricingService pricingService)
//        {
//            _mathService = mathService;
//            _apiServiceFactory = apiServiceFactory;
//            _cryptoCompareDataService = cryptoCompareDataService;
//            _configService = configService;
//            _emailService = emailService;
//            _pricingService = pricingService;
//        }

//        public ITradingService Create(string exchangeCurrentlyHoldingFunds, string primaryCoin, decimal investment)
//        {
//            return new TradingService(exchangeCurrentlyHoldingFunds, primaryCoin, investment, _mathService, _apiServiceFactory, _cryptoCompareDataService, _configService, _emailService, _pricingService);
//        }
//    }
//}