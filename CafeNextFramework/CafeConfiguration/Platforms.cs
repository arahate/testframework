using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
    [ConfigurationCollection(typeof(PlatformConfiguration), AddItemName = Constant.PLATFORM_TAG)]
   public class Platforms : ConfigurationElementCollection
    {
        
        protected override ConfigurationElement CreateNewElement()
        {
            return new PlatformConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PlatformConfiguration)(element)).AutomationPlatformName;
        }
    }
}
