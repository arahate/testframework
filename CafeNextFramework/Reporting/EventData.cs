using OpenQA.Selenium;

namespace CafeNextFramework.Reporting
{
    public class EventData
    {
        public string TestStepDescription { get; set; }
        public string TestData { get; set; }
        public string Expected { get; set; }
        public string Actual { get; set; }
        public Screenshot ScreenShotFile { get; set; }
        public string BrowserName { get; set; }
    }
}