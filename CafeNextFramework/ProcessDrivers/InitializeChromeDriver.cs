using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CafeNextFramework.ProcessDrivers
{
    class InitializeChromeDriver : IInitializeWebDriver
    {
        public List<IWebDriver> Get()
        {
            List<IWebDriver> drivers = new List<IWebDriver>();
            try
            {
              IWebDriver  webDr = new ChromeDriver();
                drivers.Add(webDr);
                return drivers;
            }
            catch (WebDriverException e)
            {
                Console.WriteLine("Not Able to launch the Chrome Driver" + e.Message);
            }
            return drivers;
        }
    }
}
