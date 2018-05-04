using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
    public class ReporterConfigurations : ConfigurationElement
    {
        [ConfigurationProperty(Constant.REF_ATTR)]
        public string ElementName
        {
            get
            {
                return (string)this[Constant.REF_ATTR];
            }
        }
    }
}