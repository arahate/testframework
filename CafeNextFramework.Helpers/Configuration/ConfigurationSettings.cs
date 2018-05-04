using CafeNextFramework.TestAccess;
using System;
using System.Configuration;

namespace CafeNextFramework.Helpers.Configuration
{
    public static class ConfigurationSettings
    {
        public static string Get(string pathName, MasterSheetRow masterSheetRow, string country = null, string replaceWith = null)
        {
            if (masterSheetRow != null)
            {
                try
                {
                    string xpathValue = (!string.IsNullOrEmpty(country) && ConfigurationManager.AppSettings[pathName + "_" + country] != null)
                        ? ConfigurationManager.AppSettings[pathName + "_" + country] : ConfigurationManager.AppSettings[pathName];
                    if (!string.IsNullOrEmpty(replaceWith))
                    {
                        xpathValue = xpathValue.Replace("index", replaceWith);
                    }

                    return xpathValue;
                }
                catch (ArgumentException ex)
                {
                    masterSheetRow.TestResultLogger.Failed("Find Xpath Value", "ArgumentException ocuured while Finding Xpath Value for Key :" + pathName + " from config ",
                        "Should able to get the Xpath value ",
                       "ArgumentException occured while Finding Xpath Value for Key from config " + ex.Message);
                    return null;
                }
                catch (ConfigurationErrorsException ex)
                {
                    masterSheetRow.TestResultLogger.Failed("Find Xpath Value", "ConfigurationErrorsException ocuured while Finding Xpath Value for Key :" + pathName + " from config ",
                        "Should able to get the Xpath value ",
                       "ConfigurationErrorsException occured while Finding Xpath Value for Key from config " + ex.Message);
                    return null;
                }
            }
            return null;
        }
    }
}