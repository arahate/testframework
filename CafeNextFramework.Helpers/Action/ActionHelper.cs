using CafeNextFramework.Helpers.Configuration;
using CafeNextFramework.Helpers.Verification;
using CafeNextFramework.TestAccess;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CafeNextFramework.Helpers.Action
{
    public static class ActionHelper
    {
        private const string disabledFlag = "Disabled";

        public static string DecodeFromUtf8(this string utf8Format)
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(utf8Format);
            return Encoding.UTF8.GetString(utf8Bytes);
        }

        public static IWebDriver Initialize(this IWebDriver driver, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                try
                {
                    masterSheetRow.Driver.Navigate().GoToUrl(masterSheetRow.Url);
                    if (!masterSheetRow.MobileAutomationRun)
                    {
                        masterSheetRow.Driver.Manage().Window.Maximize();
                    }
                    masterSheetRow.TestResultLogger.Passed("Page Load Application  : ", "Loading page ",
                                         "Retrieving all element from the page",
                                         "Loading page successful");
                    return masterSheetRow.Driver;
                }
                catch (WebDriverException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Page Load Application ", "WebDriverException error has occured in page load",
                                            "Retrieving all element from the page",
                                            "WebDriverException has occured, unable to process page load" + e.Message);
                }
                catch (InvalidOperationException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Page Load Application ", "InvalidOperationException error has occured in page load",
                                            "Retrieving all element from the page",
                                            "InvalidOperationException has occured, unable to process page load" + e.Message);
                }
                masterSheetRow.Driver.CloseBrowser(masterSheetRow);  //Closing the browser in case Initialize method goes to catch,so can not continue execution
            }
            return null;
        }

        public static string[] TrimHtmlTag(this string result)
        {
            string[] removeTagArray = { "b", "ul", "br", "i", "ul", "li", "ol", "font", "span", "div", "u" };
            foreach (string removeTag in removeTagArray)
            {
                string regExpressionToRemoveBeginTag = string.Format("<{0}([^>]*)>", removeTag);
                Regex regEx = new Regex(regExpressionToRemoveBeginTag, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                result = regEx.Replace(result, "");

                string regExpressionToRemoveEndTag = string.Format("</{0}([^>]*)>", removeTag);
                regEx = new Regex(regExpressionToRemoveEndTag, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                result = regEx.Replace(result, "");
            }
            result = result.Replace("\r", "");
            result = result.Replace("\"", "");
            string[] tokens = result.Split('\n');

            tokens = (from e in tokens select e.Trim()).ToArray();

            return tokens;
        }

        public static string TrimInput(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string[] temp = input.Split('-');
                if (!string.IsNullOrEmpty(temp[1]))
                {
                    return temp[1];
                }
            }
            return string.Empty;
        }

        public static void CloseBrowser(this IWebDriver driver, MasterSheetRow masterSheetRow)
        {
            if (driver != null && masterSheetRow != null)
            {
                masterSheetRow.TestResultLogger.Passed("Browser is expected to be closed", "Browser is expected to be closed",
                                      "Verify Browser should be closed successfully",
                                      "Browser is closed successfully");
                driver.Quit();
            }
        }

        public static IWebElement FindElement(this IWebDriver driver, string configurationKeyName,
            string scenarioName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (driver != null && masterSheetRow != null && !string.IsNullOrEmpty(configurationKeyName))
            {
                try
                {
                    if (configurationKeyName != disabledFlag)
                    {
                        IWebElement element = driver.FindElement(By.XPath(configurationKeyName));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", element);
                        return element;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (NoSuchElementException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, "NoSuchElementException occured, " + elementName + " element is not present",
                                          elementName + " element should present on the page",
                                          elementName + " element doesn't exist on the page <br/> Message: " + ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, "InvalidOperationException occured, " + elementName + " element is not present",
                                          elementName + " element should present on the page",
                                          elementName + " element doesn't exist on the page. InvalidOperationException occured. <br/> Message: " + ex.Message);
                }
            }
            return null;
        }

        public static IList<IWebElement> FindElements(this IWebDriver driver, string configurationKeyName,
            string scenarioName, string elementName, MasterSheetRow masterSheetRow)
        {
            IList<IWebElement> elements = new List<IWebElement>();
            if (driver != null && masterSheetRow != null && configurationKeyName != disabledFlag)
            {
                elements = driver.FindElements(By.XPath(configurationKeyName));
                if (elements.Any())
                {
                    return elements;
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, "List does not contain any " + elementName + " WebElements",
                                     "List should contains " + elementName + " WebElements",
                                     "List does not contain any WebElements");
                }
            }
            return elements;
        }

        public static IWebElement FindElementWithTimeOut(this IWebDriver driver, string configurationKeyName, string scenarioName, string elementName,
            MasterSheetRow masterSheetRow, int timeoutInSeconds = 20)
        {
            if (masterSheetRow != null)
            {
                try
                {
                    if (driver != null && configurationKeyName != disabledFlag)
                    {
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                        return wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(configurationKeyName)));
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (WebDriverTimeoutException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Find Element " + scenarioName + " > " + elementName,
                        "Web Driver Timeout Exception Occurred <br/>" + elementName + " element not found within  " + timeoutInSeconds + " second",
                        elementName + " Element should be present",
                       "WebDriverTimeoutException Occurred while locating the element<br/> Message: " + e.Message);
                }
                catch (ArgumentException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Find Element " + scenarioName + " > " + elementName,
                        "ArgumentException Occurred while locating the " + elementName + " element",
                         elementName + " Element should be present",
                        "ArgumentException exception Occurred while locating the element<br/> Message: " + e.Message);
                }
            }
            return null;
        }

        public static IList<IWebElement> FindElementsWithTimeOut(this IWebDriver driver, string configurationKeyName, string scenarioName,
            string elementName, MasterSheetRow masterSheetRow, int timeoutInSeconds = 20)
        {
            IList<IWebElement> elements = new List<IWebElement>();
            if (masterSheetRow != null)
            {
                try
                {
                    if (driver != null && configurationKeyName != disabledFlag)
                    {
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                        elements = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(configurationKeyName)));
                        if (elements.Any())
                        {
                            return elements;
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, "List does not contain any " + elementName + " WebElements",
                                             "List should contains " + elementName + " WebElements",
                                             "List does not contain any WebElements");
                        }
                    }
                }
                catch (WebDriverTimeoutException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Find Element " + scenarioName + " > " + elementName,
                        "Web Driver Timeout Exception Occurred <br/>" + elementName + " elements not found within  " + timeoutInSeconds + " second",
                         elementName + " Elements should be present",
                        "WebDriverTimeoutException Occurred while locating the elements<br/> Message: " + e.Message);
                }
                catch (ArgumentException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Find Element " + scenarioName + " > " + elementName,
                         "ArgumentException Occurred while locating the " + elementName + " elements",
                         elementName + " Elements should be present",
                        "ArgumentException Occurred while locating the elements<br/> Message: " + e.Message);
                }
            }
            return elements;
        }

        public static string GetAttribute(this IWebElement element, string attributeName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (element != null && masterSheetRow != null)
            {
                try
                {
                    string elementAttribute = element.GetAttribute(attributeName);
                    if (!string.IsNullOrEmpty(elementAttribute))
                    {
                        masterSheetRow.TestResultLogger.Passed("Get Element Attribute",
                                         "Get " + attributeName + " Attribute from " + elementName + "<br/>" +
                                          attributeName + " Attribute value is: " + elementAttribute,
                                        "Should able to get " + attributeName + "Attribute value from " + elementName,
                                        "Got the " + attributeName + "Attribute value from " + elementName);
                    }
                    return elementAttribute;
                }
                catch (StaleElementReferenceException ex)
                {
                    masterSheetRow.TestResultLogger.Failed("Get Element Attribute",
                                          "StaleElementReferenceException occured while getting element Attribute",
                                          "Should able to get the element attribute",
                                          "StaleElementReferenceException occured while getting element Attribute" + ex.Message);
                }
            }
            return null;
        }

        public static Uri GetElementUrl(this IWebElement element, string attributeName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                try
                {
                    string url = element.GetAttribute(attributeName, elementName, masterSheetRow);
                    if (!string.IsNullOrEmpty(url))
                    {
                        return new Uri(url);
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed("Element " + elementName + "Url",
                                            "element Url is not Present,Unable to get Url from " + elementName,
                                            "Should able to get the Url From Element",
                                            "Unable to get Proper Url from element");
                    }
                }
                catch (UriFormatException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Element " + elementName + "Url",
                                          "UriFormatException Ocuured,Unable to get Proper Url from " + elementName,
                                          "Should able to get the Url From Element",
                                          "UriFormatException Ocuured, Unable to get Proper Url from element" + e.Message);
                }
            }
            return null;
        }

        public static bool Click(this IWebElement element, string scenarioName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (element != null && masterSheetRow != null)
            {
                try
                {
                    if (element.Enabled)
                    {
                        element.Click();
                        masterSheetRow.TestResultLogger.Passed(scenarioName + " > " + elementName, elementName + " is clicked",
                                              elementName + " element should be clicked",
                                              elementName + " element is clicked");
                        return true;
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed(scenarioName + "> " + elementName, elementName + " Element is not enabled",
                                                                      elementName + " element should be clicked",
                                                                      elementName + " element is not clicked");
                    }
                    return false;
                }
                catch (ElementNotVisibleException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + "> " + elementName,
                                               "ElementNotVisibleException occured while clicking " + elementName,
                                                elementName + " element should be clicked",
                                               "ElementNotVisibleException occured while clicking " + ex.Message);
                }
                catch (StaleElementReferenceException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + "> " + elementName,
                                               "StaleElementReferenceException occured while clicking " + elementName,
                                                elementName + " element should be clicked",
                                               "StaleElementReferenceException occured while clicking " + ex.Message);
                }
                catch (InvalidOperationException ioe)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + "> " + elementName,
                                               "InvalidOperationException occured while clicking " + elementName,
                                                elementName + " element should be clicked",
                                               "InvalidOperationException occured while clicking " + ioe.Message);
                }
            }
            return false;
        }

        public static void SendKeys(this IWebElement element, string sendKeyData, string pageName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (element != null && masterSheetRow != null)
            {
                try
                {
                    element.Clear();
                    if (ConfigurationSettings.Get(sendKeyData, masterSheetRow) != null)
                    {
                        element.SendKeys(ConfigurationSettings.Get(sendKeyData, masterSheetRow));
                        masterSheetRow.TestResultLogger.Passed(pageName + " > " + elementName,
                                             "Enter " + elementName + " :" + ConfigurationSettings.Get(sendKeyData, masterSheetRow) + " in Text Box ",
                                              elementName + " should be entered in text box",
                                              elementName + " is Entered in Text Box");
                    }
                    else
                    {
                        element.SendKeys(sendKeyData);
                        masterSheetRow.TestResultLogger.Passed(pageName + " > " + elementName,
                                             "Enter " + elementName + " :" + sendKeyData + " in Text Box ",
                                              elementName + " should be entered in text box",
                                              elementName + " is Entered in Text Box");
                    }
                }
                catch (InvalidElementStateException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName,
                                         "InvalidElementStateException occured, not able to enter name in Text Box ",
                                          elementName + " should be entered in text box",
                                          elementName + " is not entered in Text Box" + ex.Message);
                }
                catch (ElementNotVisibleException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName,
                                         "ElementNotVisibleException occured, not able to enter name in Text Box ",
                                          elementName + " should be entered in text box",
                                          elementName + " is not entered in Text Box" + ex.Message);
                }
                catch (StaleElementReferenceException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName,
                                          "StaleElementReferenceException occured, not able to enter name in Text Box ",
                                           elementName + " should be entered in text box",
                                           elementName + " is not entered in Text Box" + ex.Message);
                }
            }
        }

        public static void CloseCookiePopUp(this IWebDriver driver, string pageName, string cookieConfigurationKey, string cookieCloseButtonConfigurationKey, MasterSheetRow masterSheetRow)
        {
            if (driver != null && masterSheetRow != null)
            {
                bool isCookieDivPresent = false;
                IWebElement cookieDiv;
                try
                {
                    cookieDiv = driver.FindElement(By.XPath(ConfigurationSettings.Get(cookieConfigurationKey, masterSheetRow, masterSheetRow.TestResultLogger.Country)));
                    if (cookieDiv.IsDisplayed(pageName, "Cookie message", masterSheetRow))
                    {
                        cookieDiv.IsElementTextNotNullOrEmpty(pageName, "Cookie message", masterSheetRow);
                        isCookieDivPresent = true;
                    }
                }
                catch (NoSuchElementException) //In this case it needs to passed
                {
                    masterSheetRow.TestResultLogger.Passed(pageName + " > Cookie message ", "Cookie pop up message is Handled",
                                        "Cookie message should not be present on the page",
                                        "Cookie message doesn't exist on the page");
                }
                if (isCookieDivPresent)
                {
                    TryClick(driver, cookieCloseButtonConfigurationKey, pageName, "Cookie close Button", masterSheetRow);
                    try
                    {
                        cookieDiv = driver.FindElement(By.XPath(ConfigurationSettings.Get(cookieConfigurationKey, masterSheetRow, masterSheetRow.TestResultLogger.Country)));
                        if (cookieDiv.IsDisplayed(pageName, "Cookeie message", masterSheetRow))
                        {
                            masterSheetRow.TestResultLogger.Failed(pageName + " > Cookie message ", "Cookie pop up message still displayed on page",
                                            "Cookie message should not be present on the page",
                                            "Cookie message is present on the page");
                        }
                    }
                    catch (NoSuchElementException) //In this case it needs to passed
                    {
                        masterSheetRow.TestResultLogger.Passed(pageName + " > Cookie message ", "Cookie pop up message is Not Present",
                                            "Cookie message should not be present on the page",
                                            "Cookie message doesn't exist on the page");
                    }
                }
            }
        }

        public static bool TryClick(this IWebDriver driver, string configurationKeyName, string scenarioName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                if (driver != null)
                {
                    IWebElement element = driver.FindElementWithTimeOut(ConfigurationSettings.Get
                        (configurationKeyName, masterSheetRow, masterSheetRow.TestResultLogger.Country), scenarioName, elementName, masterSheetRow);
                    if (driver.IsElementExists(masterSheetRow, element) && element.IsElementTextNotNullOrEmpty(scenarioName, elementName, masterSheetRow))
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", element);
                        element.Click(scenarioName, elementName + " link ", masterSheetRow);
                        return true;
                    }
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName,
                                             "Web Driver object is null," + elementName + " not clicked",
                                              elementName + " element should be clicked",
                                             "WebDriver Value is Null and hence Execution is not successful");
                }
            }

            return false;
        }

        public static void PlayVideo(this IWebDriver driver, IWebElement videoElement, VideoConfiguration videoConfigurationKeys, MasterSheetRow masterSheetRow)
        {
            if (driver != null && masterSheetRow != null && driver.IsElementExists(masterSheetRow, videoElement) && videoConfigurationKeys != null)
            {
                WebDriverWait waitForVideoTime = new WebDriverWait(driver, TimeSpan.FromSeconds(7));
                Actions action = new Actions(driver);
                string time;
                bool isLongVideo = false;
                IWebElement videoPopOutPlayer = null;
                try
                {
                    IJavaScriptExecutor javaScript = (IJavaScriptExecutor)driver;

                    javaScript.ExecuteScript("arguments[0].scrollIntoView();", videoElement);
                    javaScript.ExecuteScript("window.scrollBy(0,-200)", videoElement);

                    // Click on the Video Element
                    videoElement.Click("Video", "Video Play Button", masterSheetRow);

                    // Find the Pop-Out Video Player
                    CheckVideoState(driver, videoConfigurationKeys.PopOutVideoPlayerConfigurationKey, "Pop-Out Video Player", masterSheetRow);

                    // Make sure the players state is "idle"
                    CheckVideoState(driver, videoConfigurationKeys.VideoPlayerIdleConfigurationKey, "Video Player idle state", masterSheetRow);

                    IWebElement videoPlayButton = driver.FindElementWithTimeOut(ConfigurationSettings.Get
                        (videoConfigurationKeys.VideoPlayButtonConfigurationKey, masterSheetRow, masterSheetRow.TestResultLogger.Country),
                        "Video", "Video Play Button", masterSheetRow);

                    videoPlayButton.Click("Video", "Video Play Button", masterSheetRow);

                    // Verify the player starts playing
                    CheckVideoState(driver, videoConfigurationKeys.VideoPlayingConfigurationKey, "Video Playing", masterSheetRow, "play");

                    videoPopOutPlayer = driver.FindElement(ConfigurationSettings.Get
                        (videoConfigurationKeys.VideoPopupPlayerConfigurationKey, masterSheetRow, masterSheetRow.TestResultLogger.Country), "Video", "Video PopOut Player", masterSheetRow);
                    if (driver.IsElementExists(masterSheetRow, videoPopOutPlayer))
                    {
                        action.MoveToElement(videoPopOutPlayer).Build().Perform();
                        IWebElement videoTimeElement = waitForVideoTime.Until(ExpectedConditions.ElementIsVisible(By.XPath(ConfigurationSettings.Get
                            (videoConfigurationKeys.VideoTotalTimeConfigurationKey, masterSheetRow, masterSheetRow.TestResultLogger.Country))));
                        time = videoTimeElement.Text;
                        if (videoTimeElement.Displayed && !string.IsNullOrEmpty(time))
                        {
                            masterSheetRow.TestResultLogger.Passed("Video Time element", "Video Total Time is : " + time,
                                                            "Video total time should Present",
                                                            "Video total time is Present");
                            time = time.Split(':').First(); //Time In Minutes
                        }
                        else
                        {
                            time = GetVideoTimeThroughJavascript(driver, masterSheetRow);
                        }
                        PauseVideo(driver, time, isLongVideo, videoPopOutPlayer, videoConfigurationKeys, masterSheetRow);
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    string videoTime = GetVideoTimeThroughJavascript(driver, masterSheetRow);
                    PauseVideo(driver, videoTime, isLongVideo, videoPopOutPlayer, videoConfigurationKeys, masterSheetRow);
                }
            }
        }

        private static string GetVideoTimeThroughJavascript(IWebDriver driver, MasterSheetRow masterSheetRow)
        {
            if (driver != null && masterSheetRow != null)
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                string time = jse.ExecuteScript("return " + "jwplayer().getDuration();").ToString();
                if (!string.IsNullOrEmpty(time))
                {
                    masterSheetRow.TestResultLogger.Passed("Video Time", "Video Total Time is: " + time + " sec.",
                        "Video Total Time Should Present",
                        "Video Total Time is Present");
                    string videoTime = time.Split(',').First(); //Time in Seconds
                    return (Convert.ToInt32(videoTime) / 60).ToString();
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed("Video Time", "Unable to get Video Total Time",
                        "Should able to get Video Total Time ",
                        "Unable to get Video Total Time");
                }
            }
            return string.Empty;
        }

        private static void PauseVideo(IWebDriver driver, string time, bool isLongVideo, IWebElement videoPopOutPlayer, VideoConfiguration videoConfigurationKeys, MasterSheetRow masterSheetRow)
        {
            bool isVideoPopOutPlayerExists = false;
            if (!string.IsNullOrEmpty(time) && Convert.ToInt32(time) > 0)
            {
                isLongVideo = true;
            }
            isVideoPopOutPlayerExists = driver.IsElementExists(masterSheetRow, videoPopOutPlayer);
            //if total time of video is grater then 60 seconds then only pause the video
            if (isLongVideo && isVideoPopOutPlayerExists && !string.IsNullOrEmpty(videoConfigurationKeys.VideoPauseConfigurationKey))
            {
                // Click the pop-out player
                videoPopOutPlayer.Click("Video", "Video PopOut Player", masterSheetRow);
                // Verify the video is paused
                CheckVideoState(driver, videoConfigurationKeys.VideoPauseConfigurationKey, "Video Paused", masterSheetRow, "pause");
            }
            // Find the "Close" button of the pop-out player
            CloseVideo(driver, videoConfigurationKeys.VideoCloseConfigurationKey, masterSheetRow);
        }

        private static void CloseVideo(IWebDriver driver, string videoCloseConfigurationKey, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null && driver != null && !string.IsNullOrEmpty(videoCloseConfigurationKey))
            {
                TryClick(driver, videoCloseConfigurationKey, "Video", "Video Close Button", masterSheetRow);
            }
        }

        public static void CheckVideoState(IWebDriver driver, string videoXPath, string message, MasterSheetRow masterSheetRow, string action = null)
        {
            if (driver != null && masterSheetRow != null)
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 40));
                IList<IWebElement> videoBuffering;
                try
                {
                    videoBuffering = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(ConfigurationSettings.Get(videoXPath, masterSheetRow))));
                    if (videoBuffering.Count > 0)
                    {
                        videoBuffering[0].IsDisplayed("Video", message, masterSheetRow);
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    if (!string.IsNullOrEmpty(action))
                    {
                        jse.ExecuteScript("jwplayer()." + action + "();");
                        videoBuffering = driver.FindElementsWithTimeOut(ConfigurationSettings.Get(videoXPath, masterSheetRow), "Video Play", "Video element", masterSheetRow, 30);
                        if (videoBuffering.Count > 0 && videoBuffering[0].IsDisplayed("Video", message, masterSheetRow))
                        {
                            masterSheetRow.TestResultLogger.Passed("Video" + " > " + action, "Video " + action + " is Successful With JavaScript",
                                                 "Video " + action + " should Successful With JavaScript",
                                                 "Video " + action + " is Successful With JavaScript");
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Failed("Video" + " > " + action, "Video " + action + " is not Successful With JavaScript",
                                                                         "Video " + action + " should Successful With JavaScript",
                                                                         "Video " + action + " is not Successful With JavaScript");
                        }
                    }
                }
            }
        }

        public static void ClickCarouselArrow(this IWebDriver driver, string carouselImageConfigurationKey, string pageName, MasterSheetRow masterSheetRow)
        {
            IWebElement tempImage;

            #region Next Button Click

            IWebElement carouselImage = driver.FindElement(ConfigurationSettings.Get(carouselImageConfigurationKey, masterSheetRow), pageName, "carousel Image", masterSheetRow);
            IWebElement carouselRightArrow = driver.FindElement(ConfigurationSettings.Get("FemCare_carouselNextArrow", masterSheetRow), pageName, "Carousel Right Arrow", masterSheetRow);
            if (carouselRightArrow.Click("Carousel Image", "Carousel Right Arrow", masterSheetRow))
            {
                tempImage = carouselImage;
                //Reinitialize Image element after operation to compare with previous/next image
                carouselImage = driver.FindElement(ConfigurationSettings.Get(carouselImageConfigurationKey, masterSheetRow), pageName, "carousel Image", masterSheetRow);
                tempImage.CompareCarouselImages(carouselImage, 1, masterSheetRow);
            }

            #endregion Next Button Click

            #region Previous Button Click

            IWebElement carouselLeftArrow = driver.FindElement(ConfigurationSettings.Get("FemCare_carouselPreviousArrow", masterSheetRow), pageName, "Carousel Left Arrow", masterSheetRow);
            if (carouselLeftArrow.Click("Carousel Image", "Carousel Left Arrow", masterSheetRow))
            {
                tempImage = carouselImage;
                //Reinitialize Image element after operation to compare with previous/next image
                carouselImage = driver.FindElement(ConfigurationSettings.Get(carouselImageConfigurationKey, masterSheetRow), pageName, "carousel Image", masterSheetRow);
                tempImage.CompareCarouselImages(carouselImage, 2, masterSheetRow);
            }

            #endregion Previous Button Click
        }

        public static void DragAndDropCarouselImage(this IWebDriver driver, string carouselImageConfigurationKey, string pageName, MasterSheetRow masterSheetRow)
        {
            IWebElement tempImage;
            IWebElement carouselImage = driver.FindElement(ConfigurationSettings.Get(carouselImageConfigurationKey, masterSheetRow), pageName, "carousel Image", masterSheetRow);
            if (driver != null && masterSheetRow != null)
            {
                Actions dragger = new Actions(driver);

                #region Right Side Drag and Drop

                int width = carouselImage.Size.Width;
                int height = carouselImage.Size.Height;
                dragger.ClickAndHold(carouselImage).DragAndDropToOffset(carouselImage, -(width / 4), -(height / 12)).Release().Build().Perform();//Move Right
                masterSheetRow.TestResultLogger.Passed("Carousel Image", "Carousel image moved towards Right Side",
                                                       "Carousel image should move towards Right Side",
                                                       "Carousel image moved towards Right Side");
                tempImage = carouselImage;
                //Reinitialize Image element after operation to compare with previous/next image
                carouselImage = driver.FindElement(ConfigurationSettings.Get(carouselImageConfigurationKey, masterSheetRow), pageName, "carousel Image", masterSheetRow);
                tempImage.CompareCarouselImages(carouselImage, 1, masterSheetRow);

                #endregion Right Side Drag and Drop

                #region Left Side Drag and Drop

                width = carouselImage.Size.Width;
                height = carouselImage.Size.Height;
                dragger.ClickAndHold(carouselImage).DragAndDropToOffset(carouselImage, (width / 4), -(height / 12)).Release().Build().Perform();//Move Right
                masterSheetRow.TestResultLogger.Passed("Carousel Image", "Carousel image moved towards Left Side",
                                                       "Carousel image should move towards Left Side",
                                                       "Carousel image moved towards Left Side");
                tempImage = carouselImage;
                //Reinitialize Image element after operation to compare with previous/next image
                carouselImage = driver.FindElement(ConfigurationSettings.Get(carouselImageConfigurationKey, masterSheetRow), pageName, "carousel Image", masterSheetRow);
                tempImage.CompareCarouselImages(carouselImage, 2, masterSheetRow);

                #endregion Left Side Drag and Drop
            }
        }

        public static void TryNavigateBack(this IWebDriver driver, string navigateBackButtonConfigurationKey, MasterSheetRow masterSheetRow)
        {
            if (driver != null && masterSheetRow != null)
            {
                IJavaScriptExecutor javaScript = (IJavaScriptExecutor)driver;
                string pageurl = driver.Url;
                IWebElement NavigateBackButton = driver.FindElement(ConfigurationSettings.Get(navigateBackButtonConfigurationKey, masterSheetRow),
                    "Navigate Back", "Navigate Back Button", masterSheetRow);

                if (NavigateBackButton.IsElementTextNotNullOrEmpty("Navigate to Back ", "Navigate to Back", masterSheetRow))
                {
                    Uri url = NavigateBackButton.GetElementUrl("href", "Navigate Back Button", masterSheetRow);
                    if (url.CheckBrokenUrl(masterSheetRow))
                    {
                        try
                        {
                            NavigateBackButton.Click();
                            if (pageurl.Equals(driver.Url))//Selenium click is not working hence using javascript click
                            {
                                javaScript.ExecuteScript("arguments[0].click();", NavigateBackButton);
                                masterSheetRow.TestResultLogger.Passed(pageurl + " > Navigate to Back", "Navigate to Back is clicked",
                                               "Navigate to Back element should be clicked",
                                              " Navigate to Back link is clicked by Javascript Click");
                            }
                            else
                            {
                                masterSheetRow.TestResultLogger.Passed(pageurl + " > Navigate to Back", "Navigate to Back is clicked",
                                              "Navigate to Back element should be clicked",
                                             " Navigate to Back link is clicked by Selenium Click");
                            }
                        }
                        catch (ElementNotVisibleException)//Unknown exception type while clicking navigate back button
                        {
                            javaScript.ExecuteScript("arguments[0].click();", NavigateBackButton);
                            masterSheetRow.TestResultLogger.Passed(pageurl + " > Navigate to Back", "Navigate to Back is clicked",
                                                "Navigate to Back element should be clicked",
                                               " Navigate to Back link is clicked by Javascript Click");
                        }
                        catch (StaleElementReferenceException)//Unknown exception type while clicking navigate back button
                        {
                            masterSheetRow.TestResultLogger.Failed(pageurl + " > Navigate to Back",
                                                       "StaleElementReferenceException occured,Unable to Navigate Back",
                                                       "Navigate to Back element should be clicked",
                                                       "StaleElementReferenceException occured in Navigate to Back");
                        }

                        string CurrentPageUrl = driver.Url;
                        if (pageurl.Contains(CurrentPageUrl)) //verifying url of both page url(Before navigate Back and after navigate Back)
                        {
                            masterSheetRow.TestResultLogger.Passed("Navigate to Back : ", "Navigate back to desired page : " + driver.Title,
                                                      "Should be able to navigate back to desired page ",
                                                      "Navigated back to desired page ");
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Passed("Navigate to Back  : ", "Navigate back to desired page is not successful",
                                                       "Should be able to navigate back to desired page ",
                                                       "Not navigated back to desired page ");
                        }
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed("Navigate to Back : ", "Navigate to Back link is broken ",
                                                  "Navigate to Back link should not broken",
                                                  "Navigate to Back link is broken");
                    }
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed("Navigate to Back : ", "Navigate to Back link is not present  ",
                                              "Verify Navigate to Back  is present on the page",
                                              "Navigate to Back doesn't exist on the page");
                }
            }
        }

        public static bool CheckBrokenUrl(this Uri url, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                try
                {
                    if (url != null)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        var response = (HttpWebResponse)request.GetResponse();
                        response.Close();
                        if ((int)response.StatusCode == 200)
                        {
                            Console.WriteLine("This Url : " + url + " Response Code is " + (int)response.StatusCode);

                            masterSheetRow.TestResultLogger.Passed("Verifying HttpURLConnection :",
                                                 "Verifying HttpURLConnection for Url : " + url,
                                                 "Retrieved Http Connection in the page",
                                                 "Verified HttpURLConnection successfully with status code as :" + (int)response.StatusCode);
                            return true;
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Failed("Verifying HttpURLConnection :",
                                                  "Verifying HttpURLConnection for Url : " + url,
                                                  "Retrieved HttpURLConnection in the page",
                                                  "Verified HttpURLConnection failed with status code as :" + (int)response.StatusCode);
                            return false;
                        }
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed("Verifying Url ",
                                             "Element does not have any URL  ",
                                             "Retrieved Url from the page",
                                             "Url is not present");
                    }
                    return false;
                }
                catch (System.Security.SecurityException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Verifying HttpURLConnection : ",
                                         "Security Exception found in HttpURLConnection for Url : " + url,
                                         "Retrieved HttpURLConnection in the page",
                                         "Unable to process HttpURLConnection with exception detail as :" + e.Message);

                    return false;
                }
                catch (WebException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Verifying HttpURLConnection : ",
                                          "Web Exception found in HttpURLConnection for Url : " + url,
                                          "Retrieved HttpURLConnection in the page",
                                          "Unable to process HttpURLConnection with exception detail as :" + e.Message);
                    return false;
                }
            }
            return false;
        }
    }
}