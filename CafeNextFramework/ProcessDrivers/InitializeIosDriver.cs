using CafeNextFramework.CafeConfiguration;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;

namespace CafeNextFramework.ProcessDrivers
{
    class InitializeIosDriver : IInitializeWebDriver
    {
        public List<IWebDriver> Get()
        {
            DesiredCapabilities capabilities = new DesiredCapabilities();
            Platforms platforms = CafeNextConfiguration.Platforms;
            List<IWebDriver> drivers = new List<IWebDriver>();
            foreach (PlatformConfiguration platform in platforms)
            {
                if (!string.IsNullOrEmpty(platform.AutomationPlatformName) && 
                    platform.AutomationPlatformName.Equals(Constant.IOS_DRIVER,StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        capabilities.SetCapability(MobileCapabilityType.DeviceName, platform.DeviceName);
                        capabilities.SetCapability(MobileCapabilityType.BrowserName, platform.BrowserName);
                        capabilities.SetCapability(MobileCapabilityType.PlatformVersion, platform.PlatformVersion);
                        capabilities.SetCapability(MobileCapabilityType.PlatformName, platform.PlatformName);
                        capabilities.SetCapability(MobileCapabilityType.AutomationName, platform.AutomationName);
                        IWebDriver webDr = new RemoteWebDriver(new Uri(platform.Uri.ToString()), capabilities, TimeSpan.FromSeconds(150));
                        drivers.Add(webDr);
                        return drivers;
                    }
                    catch (WebDriverException e)
                    {
                        Console.WriteLine("Not Able to launch the iOS Driver" + e.Message);
                    }
                }
            }
            return drivers;
        }
    }
}
   
