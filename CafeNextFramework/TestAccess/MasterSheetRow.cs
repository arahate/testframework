using OpenQA.Selenium;

namespace CafeNextFramework.TestAccess
{
    public class MasterSheetRow
    {
        public string TestCaseName { get; set; }
        public string BrowserName { get; set; }
        public System.Uri Url { get; set; }
        public TestResultLogger TestResultLogger { get; set; }
        public IWebDriver Driver { get; set; }
        public bool MobileAutomationRun { get; set; }
    }
}