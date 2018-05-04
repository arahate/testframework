using CafeNextFramework.CafeConfiguration;
using CafeNextFramework.Reporting;
using CafeNextFramework.TestAccess;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CafeNextFramework
{
    public class ScriptRunner
    {
        public List<Thread> threadList = new List<Thread>();

        public void Run()
        {
            try
            {
                string excelPath = CafeNextConfiguration.ExcelFilePath;
                Markets markets = CafeNextConfiguration.Markets;
                if (markets.Count > 0 && !string.IsNullOrEmpty(excelPath))
                {
                    string excelAccess = Path.GetFullPath(excelPath);
                    foreach (MarketConfiguration market in markets)
                    {
                        Thread countryThread = new Thread(new ParameterizedThreadStart(StartThread));
                        if (!string.IsNullOrEmpty(Convert.ToString(market.Url)) && !string.IsNullOrEmpty(market.Country))
                        {
                            threadList.Add(countryThread);
                            countryThread.Start(excelAccess + "~" + market.Country + "~" + market.Url);
                        }
                        else
                        {
                            Console.WriteLine("Country Name or Url is not present");
                        }
                    }
                    ConsolidatedScriptReport();
                }
                else
                {
                    Console.WriteLine("Market Configuration Element or Excel Path is not present");
                }
            }
            catch (IndexOutOfRangeException ioore)
            {
                throw new CafeNextFrameworkException(ioore.Message, ioore);
            }
            catch (ArgumentNullException ane)
            {
                throw new CafeNextFrameworkException(ane.Message, ane);
            }
            catch (ArgumentException ae)
            {
                throw new CafeNextFrameworkException(ae.Message, ae);
            }
            catch (TypeInitializationException te)
            {
                throw new CafeNextFrameworkException(string.Format("Unable to get the configuration section Group {0} ", te.Message), te);
            }
        }

        private void StartThread(object allDetails)
        {
            string allDetailsStore = (string)allDetails;
            string[] tempStore = allDetailsStore.Split('~');

            if (tempStore.Length.Equals(3))
            {
                Console.WriteLine("Thread Started for country name : {0}  with url : {1}", tempStore[1], tempStore[2]);
                var masterSheetRows = new ExcelTestSetAccess().Scripts;
                foreach (MasterSheetRow row in masterSheetRows)
                {
                    try
                    {
                        if(row.BrowserName.Equals(Constant.ANDROID_DRIVER,StringComparison.OrdinalIgnoreCase)|| 
                            row.BrowserName.Equals(Constant.IOS_DRIVER, StringComparison.OrdinalIgnoreCase))
                        {
                            row.MobileAutomationRun = true;
                        }
                        WebSeleniumUiDriver seleniumUIDriver = new WebSeleniumUiDriver();
                        List<IWebDriver> drivers;
                        drivers = seleniumUIDriver.LaunchApplication(row.BrowserName);
                        foreach (var driver in drivers)
                        {
                            ScriptExecution(row, driver, tempStore);
                        }
                                                  
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Argument Exception during test instance execution {0} ", e.Message);
                    }
                }
            }
        }

        private void ConsolidatedScriptReport()
        {
            if (threadList.Count > 0)
            {
                WaitUntilAllThreadsComplete(threadList);
                ConsolidatedReporter objMainReport = new ConsolidatedReporter();
                objMainReport.GenerateConsolidatedReport();
            }
        }

        private static void WaitUntilAllThreadsComplete(List<Thread> threadList)
        {
            foreach (Thread marketThread in threadList)
            {
                marketThread.Join();
            }
        }

        private void ScriptExecution(MasterSheetRow row, IWebDriver driver, string[] tempStore)
        {
            ICapabilities capabilities;
            capabilities = ((RemoteWebDriver)driver).Capabilities;
            try
            {
                row.BrowserName = capabilities.BrowserName;
                row.Driver = driver;
                row.TestResultLogger = new TestResultLogger(tempStore[1], row.TestCaseName, row.BrowserName, row.Driver);
                row.Url = new Uri(tempStore[2]);
                var testScripts = Assembly.GetEntryAssembly().GetTypes()
                             .Where(x => typeof(ITestScript).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract && x.Name.Equals
                             (row.TestCaseName)).ToList();

                if (testScripts.Any())
                {
                    ITestScript testScript = Assembly.GetEntryAssembly().CreateInstance(testScripts.FirstOrDefault().ToString()) as ITestScript;
                    testScript.ExecuteScript(row);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Unknown error during User Journey execution {0} ", e.Message);
            }
        }
    }
}