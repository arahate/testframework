using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
    [ConfigurationCollection(typeof(ReporterConfigurations), AddItemName = Constant.REPORTER_REF_TAG)]
    public class Reporters : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ReporterConfigurations();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ReporterConfigurations)(element)).ElementName;
        }
    }
}