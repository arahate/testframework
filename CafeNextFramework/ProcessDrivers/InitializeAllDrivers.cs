using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;

namespace CafeNextFramework.ProcessDrivers
{
    class InitializeAllDrivers : IInitializeWebDriver
    {
        public List<IWebDriver> Get()
        {
            List<IWebDriver> drivers = new List<IWebDriver>();
            try
            {
                InternetExplorerOptions options = new InternetExplorerOptions();
                options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                IWebDriver webDrIE = new InternetExplorerDriver(options);
                IWebDriver webDrChrome = new ChromeDriver();
                drivers.Add(webDrIE);
                drivers.Add(webDrChrome);
                return drivers;
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("Not Able to launch the Browser Driver" + e.Message);
            }
            return drivers;
        }
    }
}
