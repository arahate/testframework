using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CafeNextFramework.ProcessDrivers
{
    internal class WebDriverFactory
    {
        readonly Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
        public WebDriverFactory()
        {
            dictionary.Add(Constant.IE_DRIVER, typeof(InitializeIEDriver));
            dictionary.Add(Constant.CHROME_DRIVER, typeof(InitializeChromeDriver));
            dictionary.Add(Constant.FIREFOX_DRIVER, typeof(InitializeFirefoxDriver));
            dictionary.Add(Constant.ALL_DRIVER, typeof(InitializeAllDrivers));
            dictionary.Add(Constant.ANDROID_DRIVER, typeof(InitializeAndroidDriver));
            dictionary.Add(Constant.IOS_DRIVER, typeof(InitializeIosDriver));
        }
        public List<IWebDriver> Get(string browserName)
        {
            if (dictionary.ContainsKey(browserName))
            {
                try
                {
                    IInitializeWebDriver myObject = (IInitializeWebDriver)(Activator.CreateInstance(dictionary[browserName]));
                    return myObject.Get();
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("ArgumentException occurred, Unable to create object of web driver"+e.Message);
                }
            }
            return new List<IWebDriver>();
        }
    }
}


