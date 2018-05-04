using CafeNextFramework.ProcessDrivers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CafeNextFramework
{
    public class WebSeleniumUiDriver
    {
        public List<IWebDriver> OpenBrowser(string browser)
        {
            if (!string.IsNullOrEmpty(browser))
            {
                browser = browser.Trim().ToUpper(CultureInfo.InvariantCulture);
                WebDriverFactory driver = new WebDriverFactory();
                return driver.Get(browser);
                 
            }
            else
            {
                Console.WriteLine("Unable to get browser name");
            }
            return new List<IWebDriver>();
        }
     
        public List<IWebDriver> LaunchApplication(string browser)
        {
            return OpenBrowser(browser);
        }

    }
}