using CafeNextFramework.CafeConfiguration;
using CafeNextFramework.Reporting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace CafeNextFramework
{
    public class TestResultLogger : IDisposable
    {
        private readonly string _country;
        private readonly string _moduleName;
        private readonly string _browserName;
        private readonly IWebDriver _driver;
        private readonly List<IReporter> reporters;
        public string Country => _country;

        public string ModuleName => _moduleName;

        public TestResultLogger(string country, string moduleName, string browserName, IWebDriver driver)
        {
            _country = country;
            _moduleName = moduleName;
            _browserName = browserName;
            _driver = driver;
            reporters = CafeNextConfiguration.Reporters;
        }

        public void Passed(string testStepDescription, string testData, string expected, string actual)
        {
            if (CafeNextConfiguration.ScreenShotLevel.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                Log(testStepDescription, testData, ResultType.PASSED, expected, actual, ((ITakesScreenshot)_driver).GetScreenshot());
            }
            else
            {
                Log(testStepDescription, testData, ResultType.PASSED, expected, actual);
            }
        }

        public void Failed(string testStepDescription, string testData, string expected, string actual)
        {
            Log(testStepDescription, testData, ResultType.FAILED, expected, actual, ((ITakesScreenshot)_driver).GetScreenshot());
        }

        public void Warning(string testStepDescription, string testData, string expected, string actual)
        {
            Log(testStepDescription, testData, ResultType.WARNING, expected, actual);
        }

        public void Log(string testStepDescription, string testData, ResultType resultType, string expected,
                                     string actual)
        {
            EventData properties = new EventData
            {
                TestStepDescription = testStepDescription,
                TestData = testData,
                Expected = expected,
                Actual = actual,
                BrowserName = _browserName
            };
            foreach (IReporter item in reporters)
            {
                item.Log(resultType, properties, Country, ModuleName);
            }
        }

        public void Log(string testStepDescription, string testData, ResultType resultType, string expected,
                                     string actual, Screenshot screenShotFile)
        {
            EventData properties = new EventData
            {
                TestStepDescription = testStepDescription,
                TestData = testData,
                Expected = expected,
                Actual = actual,
                BrowserName = _browserName,
                ScreenShotFile = screenShotFile
            };

            foreach (IReporter item in reporters)
            {
                item.Log(resultType, properties, Country, ModuleName);
            }
        }

        public void Dispose()
        {
            //Dispose(true);
            GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing && reporter != null)
        //    {
        //        reporter.Dispose();
        //    }
        //}
    }
}