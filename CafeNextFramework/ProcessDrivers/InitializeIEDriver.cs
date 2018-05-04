using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace CafeNextFramework.ProcessDrivers
{
    class InitializeIEDriver : IInitializeWebDriver
    {
        public List<IWebDriver> Get()
        {
            List<IWebDriver> drivers = new List<IWebDriver>();
            try
            {
                InternetExplorerOptions options = new InternetExplorerOptions();
                options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                IWebDriver webDr = new InternetExplorerDriver(options);
                drivers.Add(webDr);
                return drivers;
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("Not Able to launch the IE Driver" + e.Message);
            }
            return drivers;
        }
    }
}
