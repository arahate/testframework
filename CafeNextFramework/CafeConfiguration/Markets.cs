using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
    [ConfigurationCollection(typeof(MarketConfiguration), AddItemName = Constant.MARKET_TAG)]
    public class Markets : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MarketConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MarketConfiguration)(element)).Country;
        }
    }
}