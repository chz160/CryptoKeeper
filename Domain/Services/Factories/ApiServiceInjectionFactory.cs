using System;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoKeeper.Domain.Services.Factories
{
    public class ApiServiceInjectionFactory : IApiServiceInjectionFactory
    {
        private readonly IConfigService _configService;
        private readonly ICryptoCompareDataService _cryptoCompareDataService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBuilderFactory _builderFactory;

        public ApiServiceInjectionFactory(IConfigService configService, ICryptoCompareDataService cryptoCompareDataService, IBuilderFactory builderFactory, IServiceProvider serviceProvider)
        {
            _configService = configService;
            _cryptoCompareDataService = cryptoCompareDataService;
            _builderFactory = builderFactory;
            _serviceProvider = serviceProvider;
        }

        public IAmAnApiService Create<T>(Exchange exchange)
        {
            return (IAmAnApiService)Activator.CreateInstance(typeof(T), exchange, _configService, _cryptoCompareDataService, _serviceProvider, _builderFactory);
        }

        public IAmAnApiService Create<T>()
        {
            return (IAmAnApiService)Activator.CreateInstance(typeof(T), _configService, _cryptoCompareDataService, _serviceProvider, _builderFactory);
        }
    }
}