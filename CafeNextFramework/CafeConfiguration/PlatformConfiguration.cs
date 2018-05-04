using System;
using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
   public class PlatformConfiguration : ConfigurationElement
    {
        [ConfigurationProperty(Constant.PLATFORMNAME)]
        public string AutomationPlatformName
        {
            get
            {
                return (string)this[Constant.PLATFORMNAME];
            }
        }
        [ConfigurationProperty(Constant.MOBILEAUTOMATION_DEVICENAME_ATTR)]
        public string DeviceName
        {
            get
            {
                return (string)this[Constant.MOBILEAUTOMATION_DEVICENAME_ATTR];
            }
        }
        [ConfigurationProperty(Constant.MOBILEAUTOMATION_BROWSERNAME_ATTR)]
        public string BrowserName
        {
            get
            {
                return (string)this[Constant.MOBILEAUTOMATION_BROWSERNAME_ATTR];
            }
        }
        [ConfigurationProperty(Constant.MOBILEAUTOMATION_PLATFORMVERSION_ATTR)]
        public string PlatformVersion
        {
            get
            {
                return (string)this[Constant.MOBILEAUTOMATION_PLATFORMVERSION_ATTR];
            }
        }
        [ConfigurationProperty(Constant.MOBILEAUTOMATION_PLATFORMNAME_ATTR)]
        public string PlatformName
        {
            get
            {
                return (string)this[Constant.MOBILEAUTOMATION_PLATFORMNAME_ATTR];
            }
        }
        [ConfigurationProperty(Constant.MOBILEAUTOMATION_AUTOMATIONNAME_ATTR)]
        public string AutomationName
        {
            get
            {
                return (string)this[Constant.MOBILEAUTOMATION_AUTOMATIONNAME_ATTR];
            }
        }
        [ConfigurationProperty(Constant.MOBILEAUTOMATION_URI_ATTR)]
        public Uri Uri
        {
            get
            {
                return (Uri)this[Constant.MOBILEAUTOMATION_URI_ATTR];
            }
        }
    }
}
