using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services.Apis.PricingMonitors
{
    public class NullMonitorService : IAmPricingMonitor
    {
        public void Monitor()
        {
            //Do nothing
        }
    }
}