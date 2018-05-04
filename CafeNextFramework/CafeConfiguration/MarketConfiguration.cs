using System;
using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
    public class MarketConfiguration : ConfigurationElement
    {
        [ConfigurationProperty(Constant.MARKET_COUNTRY_ATTR)]
        public string Country
        {
            get
            {
                return (string)this[Constant.MARKET_COUNTRY_ATTR];
            }
        }

        [ConfigurationProperty(Constant.MARKET_URL_ATTR)]
        public Uri Url
        {
            get
            {
                return (Uri)this[Constant.MARKET_URL_ATTR];
            }
        }
    }
}