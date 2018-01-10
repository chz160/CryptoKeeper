using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CryptoKeeper.Domain.Builders.Factories;
using CryptoKeeper.Domain.Builders.Interfaces;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Services.Interfaces;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IBuilderFactory _builderFactory;

        public ConfigService() : this(new BuilderFactory())
        { }

        public ConfigService(IBuilderFactory builderFactory)
        {
            _builderFactory = builderFactory;
        }

        public List<ApiConfigurationData> GetApiConfigurations()
        {
            var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(executableLocation, "api.json");
            var apiConfigText = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<ApiConfigurationData>>(apiConfigText);
        }

        public ApiConfigurationData GetApiConfigurationForExchange(string exchange)
        {
            return GetApiConfigurations().FirstOrDefault(m => m.Exchange == exchange);
        }

        public List<Exchange> GetConfiguredExchanges()
        {
            var result = new List<Exchange>();
            var configuredApis = GetApiConfigurations();
            if (configuredApis == null || !configuredApis.Any()) throw new Exception("There must be at least two configured APIs in the api.json file.");
            foreach (var api in configuredApis.Where(m=>m.Configured))
            {
                result.Add(_builderFactory.Create<ApiConfigurationData, Exchange>(api).Build());
            }
            return result;
        }
    }
}
