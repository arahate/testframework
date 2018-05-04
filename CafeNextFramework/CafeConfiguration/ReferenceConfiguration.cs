using System.Configuration;

namespace CafeNextFramework.CafeConfiguration
{
    public class ReferenceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty(Constant.REFRENCE_LogFolder_ATTR)]
        public string LogFolder
        {
            get
            {
                return (string)this[Constant.REFRENCE_LogFolder_ATTR];
            }
        }

        [ConfigurationProperty(Constant.REFRENCE_ScreenShotLevel_ATTR)]
        public string ScreenShot
        {
            get
            {
                return (string)this[Constant.REFRENCE_ScreenShotLevel_ATTR];
            }
        }

        [ConfigurationProperty(Constant.REFRENCEINPUT_TAG)]
        public ExcelConfiguration ExcelConfiguration
        {
            get
            {
                return (ExcelConfiguration)this[Constant.REFRENCEINPUT_TAG];
            }
        }

        [ConfigurationProperty(Constant.MARKETS_TAG)]
        public Markets MarketElement
        {
            get { return ((Markets)(base[Constant.MARKETS_TAG])); }
        }
        [ConfigurationProperty(Constant.PLATFORMS_TAG)]
        public Platforms PlatformElement
        {
            get { return ((Platforms)(base[Constant.PLATFORMS_TAG])); }
        }

        [ConfigurationProperty(Constant.REPORTER_TAG)]
        public Reporters ReporterTypes
        {
            get { return ((Reporters)(base[Constant.REPORTER_TAG])); }
        }

        [ConfigurationProperty(Constant.REFERENCE_CONSOLIDATEDREPORT_XSLTPATH_ATTR)]
        public string ConsolidatedReportXsltPath
        {
            get
            {
                return (string)this[Constant.REFERENCE_CONSOLIDATEDREPORT_XSLTPATH_ATTR];
            }
        }
    }
}