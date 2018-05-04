using CafeNextFramework.Reporting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace CafeNextFramework.CafeConfiguration
{
    public class CafeNextConfiguration : ConfigurationSectionGroup
    {
        public static string LogFolderPath { get; }
        public static string ExcelFilePath { get; }
        public static string ExcelSheetName { get; }
        public static string ScreenShotLevel { get; }
        public static Markets Markets { get; }
        public static List<IReporter> Reporters { get; }
        public static string ConsolidatedXsltFilePath { get; }
        public static Platforms Platforms { get; }
        static CafeNextConfiguration()
        {
            ReferenceConfiguration section = GetConfigurationSection().Reference;
            ExcelFilePath = section.ExcelConfiguration.FilePath;
            ExcelSheetName = section.ExcelConfiguration.Sheet;
            Markets = section.MarketElement;
            Platforms = section.PlatformElement;
            LogFolderPath = section.LogFolder;
            ScreenShotLevel = section.ScreenShot;
            List<XmlElement> elements = GetReporterNodes();
            Reporters = ParseReporter(elements);
            ConsolidatedXsltFilePath = section.ConsolidatedReportXsltPath;
        }

        public static Configuration GetConfig
        {
            get { return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }
        }

        [ConfigurationProperty(Constant.ROOT_TAG)]
        public ReferenceConfiguration Reference
        {
            get { return (ReferenceConfiguration)base.Sections[Constant.ROOT_TAG]; }
        }

        private static CafeNextConfiguration GetConfigurationSection()
        {
            Configuration config = GetConfig;
            CafeNextConfiguration CafeConfiguration = (CafeNextConfiguration)config.
                GetSectionGroup(Constant.SECTIONGROUP_TAG);
            return CafeConfiguration;
        }

        private static XmlDocument GetConfigXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            Configuration config = GetConfig;
            xmlDocument.Load(config.FilePath);
            return xmlDocument;
        }

        public static Dictionary<string, string> ReporterValues()
        {
            Dictionary<string, string> elementAttributes = new Dictionary<string, string>();
            List<XmlElement> elements = GetReporterNodes();
            foreach (XmlElement element in elements)
            {
                foreach (XmlElement item in element.ChildNodes)
                {
                    elementAttributes.Add(item.Name, item.GetAttribute(Constant.REPORTERTYPE_CHILD_ATTR));
                }
            }
            return elementAttributes;
        }

        private static List<XmlElement> GetReporterNodes()
        {
            XmlDocument xmlDocument = GetConfigXml();
            List<XmlElement> elements = new List<XmlElement>();
            ReferenceConfiguration reporterConfig = GetConfigurationSection().Reference;
            Reporters reporters = reporterConfig.ReporterTypes;
            foreach (ReporterConfigurations reporter in reporters)
            {
                string nodeName = reporter.ElementName;
                XmlElement element = xmlDocument.SelectSingleNode("//" + nodeName) as XmlElement;
                elements.Add(element);
            }
            return elements;
        }

        private static List<IReporter> ParseReporter(List<XmlElement> elements)
        {
            List<IReporter> reporters = new List<IReporter>();
            foreach (XmlElement element in elements)
            {
                string type = element.GetAttribute(Constant.REPORTERTYPE_ATTR);
                IReporter Reporter = (IReporter)Activator.CreateInstance(SystemInfo.GetTypeFromString(type, true, true));
                reporters.Add(Reporter);
            }
            return reporters;
        }
    }
}