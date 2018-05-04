using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace CafeNextFramework.ProcessDrivers
{
    class InitializeFirefoxDriver : IInitializeWebDriver
    {
        public List<IWebDriver> Get()
        {
            List<IWebDriver> drivers = new List<IWebDriver>();
            try
            {
               IWebDriver webDr = new FirefoxDriver();
                drivers.Add(webDr);
                return drivers;
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("Not Able to launch the Firefox Driver" + e.Message);
            }
            return drivers;
        }
    }
}
