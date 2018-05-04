using OpenQA.Selenium;
using System.Collections.Generic;

namespace CafeNextFramework.ProcessDrivers
{
    interface IInitializeWebDriver
    {
        List<IWebDriver> Get();
    }
}
