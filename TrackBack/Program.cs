using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.Services;
using CryptoKeeper.Domain.Services.Factories;
using CryptoKeeper.Domain.Services.Interfaces;
using CryptoKeeper.Entities.Pricing.Models;
using CryptoKeeper.Entities.Pricing.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.TrackBack
{
    class Program
    {
        static void Main(string[] args)
        {
            //setup our DI
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<IPricingContext, PricingContext>();
            services.AddTransient<ICryptoCompareDataService, CryptoCompareDataService>();
            services.AddTransient<IMathService, MathService>();
            services.AddTransient<IBuilderFactory, BuilderFactory>();
            services.AddTransient<IConfigService, ConfigService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ITradingService, TradingService>();
            services.AddTransient<IApiServiceInjectionFactory, ApiServiceInjectionFactory>();
            services.AddTransient<IExchangeApiServiceFactory, ExchangeApiServiceFactory>();
            services.AddSingleton<PricingService>();
            services.AddSingleton<IPricingService>(x => x.GetRequiredService<PricingService>());
            services.BuildServiceProvider();
            var serviceProvider = services.BuildServiceProvider();

            var exchangeCurrentlyHoldingFunds = ExchangeConstants.BitTrex;
            var primaryCoin = SymbolConstants.Btc;
            //var initalInvestment = 0.06463713m;
            var initalInvestment = 1.5m;
            new SplashScreenService().ShowSplashScreen();
            serviceProvider
                .GetRequiredService<ITradingService>()
                .StartProcess(exchangeCurrentlyHoldingFunds, primaryCoin, initalInvestment);
        }
    }
}
