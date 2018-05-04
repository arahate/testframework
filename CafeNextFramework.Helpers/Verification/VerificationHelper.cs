using CafeNextFramework.Helpers.Action;
using CafeNextFramework.Helpers.Configuration;
using CafeNextFramework.TestAccess;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace CafeNextFramework.Helpers.Verification
{
    public static class VerificationHelper
    {
        public static void VerifyPageTitle(this IWebDriver webDriver, Uri url, MasterSheetRow masterSheetRow)
        {
            if (webDriver != null && masterSheetRow != null)
            {
                string curTitle = null;
                try
                {
                    //Launching the Url as we need to check the title of the page
                    webDriver.Navigate().GoToUrl(url);
                    if (url.CheckBrokenUrl(masterSheetRow))
                    {
                        curTitle = webDriver.Title;
                        if ((curTitle.Contains("404")) || (curTitle.Contains("500")) || (curTitle.Contains("error"))
                            || (string.Equals(curTitle, "Connection Timed Out", StringComparison.OrdinalIgnoreCase)))
                        {
                            masterSheetRow.TestResultLogger.Failed("Failure in the page for url : " + url,
                                                    "Verifying page load and title", "Exception occurred in the page",
                                                    "Failure while checking current url : " + curTitle);
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Passed("Verifying page load and title", "Verifying page load and title",
                                                     "Verifying URL :" + url,
                                                     "Verified page load and title successfully");
                        }
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed("Failure in the page for url : " + url,
                                                "Verifying page load", "Exception occurred in the page",
                                                "Failure while doing page load");
                    }
                }
                catch (ArgumentException e)
                {
                    masterSheetRow.TestResultLogger.Failed("Verify Url: " + url + " for page load and title",
                                          "ArgumentException exception occurred in page load and title", "Page load and title should be proper for the page",
                                          "ArgumentException occurred, unable to process page load and title in the page : " + e.Message);
                }
            }
        }

        public static bool IsElementExists(this ISearchContext webDriver, MasterSheetRow masterSheetRow, IWebElement elementToCheck = null, string pathValue = "")
        {
            if (webDriver != null && masterSheetRow != null)
            {
                if (elementToCheck != null)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(pathValue))
                        {
                            IWebElement elementToFind = null;
                            elementToFind = webDriver.FindElement(By.XPath(pathValue));
                            if (elementToFind != null)
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                    catch (NoSuchElementException)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool IsDisplayed(this IWebElement element, string scenarioName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (element != null && masterSheetRow != null)
            {
                try
                {
                    if (element.Displayed)
                    {
                        masterSheetRow.TestResultLogger.Passed(scenarioName + " > " + elementName, elementName + " is Displayed on page",
                                              elementName + " should be Displayed on the page",
                                              elementName + " is Displayed on the page");
                        return true;
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, elementName + " is not Displayed on page",
                                              elementName + " should be Displayed on the page",
                                              elementName + " is not Displayed on the page");
                        return false;
                    }
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (StaleElementReferenceException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, "StaleElementReferenceException occured in " + elementName,
                                                elementName + " should Displayed on the page",
                                               "StaleElementReferenceException occured in " + elementName + "<br/> Message: " + ex.Message);
                    return false;
                }
            }
            return false;
        }

        // @Description: Check / unchecked Check box
        public static void SelectCheckBox(this IWebDriver driver, IWebElement element, MasterSheetRow masterSheetRow, String elementName, bool? isChecked)
        {
            if (masterSheetRow != null)
            {
                try
                {
                    if ((driver.IsElementExists(masterSheetRow, element) && isChecked.HasValue).IsTrue("Element", masterSheetRow))
                    {
                        //Verifying checkbox is already checked and print the report.
                        //If checkbox is not checked, then it it will select the checkbox and will print the report.
                        if (isChecked.Value)
                        {
                            if (!element.Selected && element.Enabled)
                            {
                                element.Click("Select Check box", "Select Checkbox : " + elementName, masterSheetRow);
                            }
                        }
                        //Verifying checkbox is already unchecked and print the report.
                        //If checkbox is not unchecked, then it it will uncheck the checkbox and will print the report.
                        else
                        {
                            if (element.Selected && element.Enabled)
                            {
                                element.Click("Uncheck Checkbox", "Uncheck Checkbox : " + elementName, masterSheetRow);
                            }
                        }
                    }
                }
                catch (Exception e) when (e is NoSuchElementException || e is WebDriverException)
                {
                    masterSheetRow.TestResultLogger.Failed("Select Check box", "Element does not exist or XPath is not valid for" + elementName,
                                          "Check box should be selected",
                                          "Check box is not selected" + e.Message);
                }
            }
        }

        public static bool IsElementTextNotNullOrEmpty(this IWebElement element, string scenarioName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (element != null && masterSheetRow != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(element.Text))
                    {
                        masterSheetRow.TestResultLogger.Passed(scenarioName + " > " + elementName, elementName + " text is : " + element.Text,
                                                         elementName + " text should be present on the page",
                                                         elementName + " text is present on the page");
                        return true;
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, elementName + " text is not present",
                                                         elementName + " text should be present on the page",
                                                         elementName + " text is not present on the page");
                        return false;
                    }
                }
                catch (StaleElementReferenceException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, "StaleElementReferenceException occured in " + elementName,
                                              elementName + " text should be present on the page",
                                             "StaleElementReferenceException occured in " + elementName + "<br/> Message: " + ex.Message);
                    return false;
                }
                catch (InvalidOperationException ioe)
                {
                    masterSheetRow.TestResultLogger.Failed(scenarioName + " > " + elementName, "InvalidOperationException occured in " + elementName,
                                              elementName + " text should be present on the page",
                                             "InvalidOperationException occured in " + elementName + "<br/> Message: " + ioe.Message);
                    return false;
                }
            }
            return false;
        }

        public static void VerifyBackGroundImage(this IWebElement element, string pageName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (element != null && masterSheetRow != null)
            {
                try
                {
                    string elementUrl = string.Empty;
                    elementUrl = element.GetAttribute("style", elementName, masterSheetRow); //Image is embedded inside Div tag so we are only verifying Url of image
                    if (!string.IsNullOrEmpty(elementUrl))
                    {
                        elementUrl = elementUrl.Split('"')[1]; //split the url obtained and get the url of Image
                    }
                    Uri imageUri = new Uri(elementUrl);
                    if (imageUri.CheckBrokenUrl(masterSheetRow))
                    {
                        masterSheetRow.TestResultLogger.Passed(pageName + " > " + elementName, elementName + " Image url is not broken",
                                                  elementName + " Image should not broken",
                                                  elementName + "Image url is not broken");
                    }
                    else
                    {
                        masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName, elementName + " Image url is broken",
                                                  elementName + " Image should not broken",
                                                  elementName + "Image url is broken");
                    }
                }
                catch (StaleElementReferenceException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName, "StaleElementReferenceException occured in " + elementName + " image element:",
                                                                     "Should able to get the image element attribute",
                                                                      "StaleElementReferenceException occured while getting image element Attribute" + ex.Message);
                }
                catch (ArgumentNullException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName, "ArgumentNullException occured in " + elementName + " image element:",
                                                                     "Should able to get the image element attribute",
                                                                      "ArgumentNullException occured while getting image element Attribute" + ex.Message);
                }
                catch (UriFormatException ex)
                {
                    masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName, "UriFormatException occured in " + elementName + " image element:",
                                                                     "Should able to get the image element attribute",
                                                                      "UriFormatException occured while getting image element Attribute" + ex.Message);
                }
            }
        }

        public static void VerifyElementText(this IWebDriver webDriver, string pathName, string pageName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                IWebElement element = webDriver.FindElement(ConfigurationSettings.Get(pathName, masterSheetRow, masterSheetRow.TestResultLogger.Country), pageName, elementName, masterSheetRow);
                element.IsElementTextNotNullOrEmpty(pageName, elementName, masterSheetRow);
            }
        }

        public static void VerifyElementTextAndUrl(this IWebDriver webDriver, string pathName, string pageName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                IWebElement element = webDriver.FindElement(ConfigurationSettings.Get(pathName, masterSheetRow, masterSheetRow.TestResultLogger.Country), pageName, elementName, masterSheetRow);

                if (element.IsElementTextNotNullOrEmpty(pageName, elementName, masterSheetRow))
                {
                    Uri Url = element.GetElementUrl("href", elementName, masterSheetRow);
                    Url.CheckBrokenUrl(masterSheetRow);
                }
            }
        }

        public static void VerifyElementTextAndUrl(this IWebElement element, string pageName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null && element != null && element.IsElementTextNotNullOrEmpty(pageName, elementName, masterSheetRow))
            {
                Uri Url = element.GetElementUrl("href", elementName, masterSheetRow);
                Url.CheckBrokenUrl(masterSheetRow);
            }
        }

        public static void VerifyElementTextAndClick(this IWebDriver webDriver, string pathName, string pageName, string elementName, MasterSheetRow masterSheetRow, string replaceWith = null)
        {
            if (webDriver != null && masterSheetRow != null)
            {
                IWebElement element;

                element = webDriver.FindElement(ConfigurationSettings.Get(pathName, masterSheetRow, masterSheetRow.TestResultLogger.Country,
                    replaceWith), pageName, elementName, masterSheetRow);

                if (element.IsElementTextNotNullOrEmpty(pageName, elementName, masterSheetRow))
                {
                    string pageUrl = webDriver.Url;//Get the Page Url before Click

                    Uri Url = element.GetElementUrl("href", elementName, masterSheetRow);
                    string elementText = element.Text;
                    if (Url.CheckBrokenUrl(masterSheetRow) && element.Click(pageName, elementName, masterSheetRow))
                    {
                        string currentPageUrl = webDriver.Url; //Get the Page Url after Click
                        if (currentPageUrl != pageUrl)
                        {
                            webDriver.Navigate().Back();
                            masterSheetRow.TestResultLogger.Passed(pageName + " > " + elementName,
                                                     "Current Page URL : " + currentPageUrl + "Previous Page URL : " + pageUrl,
                                                     "Action should be resulted in successful navigation",
                                                     "Action is resulted in successful navigation");
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Failed(pageName + " > " + elementName,
                                                     "Previous page Url : " + pageUrl + " Current Page URL : " + currentPageUrl,
                                                     "Verifying the Click " + elementText + " should be successfull based on page :" + currentPageUrl,
                                                     "Click on " + elementText + " is not successfull");
                        }
                    }
                }
            }
        }

        public static void VerifyImageDisplay(this IWebDriver webDriver, string pathName, string pageName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                IWebElement image = webDriver.FindElement(ConfigurationSettings.Get(pathName, masterSheetRow, masterSheetRow.TestResultLogger.Country), pageName, elementName, masterSheetRow);
                image.IsDisplayed(pageName, elementName, masterSheetRow);
                string imageUrl = image.GetAttribute("src");
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    Uri url = new Uri(imageUrl);
                    ActionHelper.CheckBrokenUrl(url, masterSheetRow);
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed("Check Image URL", elementName + " Image URL not present in" + pageName, "imageUrl should be present", "imageUrl is not present");
                }
            }
        }

        public static void VerifyImageDisplay(this IWebElement element, string pageName, string elementName, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null && pageName != null)
            {
                try
                {
                    if (element != null)
                    {
                        string imageUrl = element.GetAttribute("src");
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            Uri url = new Uri(imageUrl);
                            ActionHelper.CheckBrokenUrl(url, masterSheetRow);
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Failed("Check Image URL", elementName + " Image URL not present in" + pageName, "imageUrl should be present", "imageUrl is not present");
                        }
                    }
                }
                catch (Exception e) when (e is NoSuchElementException || e is WebDriverException)
                {
                    masterSheetRow.TestResultLogger.Failed(pageName, "Image URL present",
                                                      "Image URL should be present",
                                                      "Image URL not present and Exception occurred as:->" + e.Message);
                }
            }
        }

        public static bool VerifyGoogleAnalytics(this IWebDriver webDriver, string pageName, string googleAnalyticsTag, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                bool isPresentInScriptTag = VerifyGoogleTag(webDriver, "script", pageName, googleAnalyticsTag, masterSheetRow);
                bool isPresentInNoScriptTag = VerifyGoogleTag(webDriver, "noscript", pageName, googleAnalyticsTag, masterSheetRow);

                if (isPresentInScriptTag && isPresentInNoScriptTag)
                {
                    masterSheetRow.TestResultLogger.Passed(pageName + " > Google Analytics",
                                        "Html indicative of Google tag manager is present in Script and NoScript Tag with identifier as :"
                                        + ConfigurationSettings.Get(googleAnalyticsTag, masterSheetRow),
                                        "Page should contain html indicative of Google tag manager in Script and NoScript Tag",
                                        "Page contains html indicative of Google tag manager in Script and NoScript Tag");
                    return true;
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed(pageName + " > Google Analytics",
                                        "Html indicative of Google tag manager is not present in Script or NoScript Tag with identifier as :"
                                        + ConfigurationSettings.Get(googleAnalyticsTag, masterSheetRow),
                                        "Page should contain html indicative of Google tag manager in Script and NoScript Tag",
                                        "Page does not contains html indicative of Google tag manager in Script or NoScript Tag");
                }
            }
            return false;
        }

        /// <summary>
        /// Verify given condition is true or false and according to it log it in Report
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="actualCondition"></param>
        /// <param name="expectedCondition"></param>
        /// <param name="masterSheetRow"></param>
        /// <returns></returns>
        public static bool IsTrue(this bool statement, string description, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                if (statement)
                {
                    masterSheetRow.TestResultLogger.Passed("Check Condition", description + " is present",
                        description + "should be present", description + "- is present");
                    return true;
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed("Check Condition", description + "- is not present",
                        description + "should be present", description + "- is not present");
                }
            }
            return false;
        }

        /// <summary>
        /// Verify Expected text contains Actual text or equal to it
        /// </summary>
        /// <param name="expectedText">Expected text is text what we are comparing it with</param>
        /// <param name="actualText">Actual text will be the text comming from UI</param>
        /// <param name="isExactMatch">Expected text contains Actual text or equal to it</param>
        /// <param name="masterSheetRow"></param>
        /// <returns></returns>
        public static bool VerifyTextEquals(this string expectedText, string actualText, bool isExactMatch, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null)
            {
                if (!string.IsNullOrEmpty(actualText) && !string.IsNullOrEmpty(expectedText))
                {
                    if (!isExactMatch)
                    {
                        if ((actualText.ToUpperInvariant().Contains(expectedText.ToUpperInvariant())))
                        {
                            masterSheetRow.TestResultLogger.Passed("Verifying Actual Text with Expected Text",
                            "Expected Text " + expectedText + " contains in Actual Text " + actualText,
                            "Actual Text should contains the expected text ",
                            "Actual Text contains the expected text");
                            return true;
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Failed("Verifying Actual Text with Expected Text",
                           "Expected Text " + expectedText + " does not contains in Actual Text " + actualText,
                           "Actual Text should contains the expected text ",
                           "Actual Text does not contains the expected text");
                        }
                    }
                    else
                    {
                        if (actualText.Equals(expectedText, StringComparison.OrdinalIgnoreCase))
                        {
                            masterSheetRow.TestResultLogger.Passed("Verifying Actual Text with Expected Text",
                            "Expected text " + expectedText + " is equal to actual text " + actualText,
                            "Actual Text should be equal to expected text ",
                            "Actual Text  is equal to expected text");
                        }
                        else
                        {
                            masterSheetRow.TestResultLogger.Failed("Verifying Actual Text with Expected Text",
                            "Expected text " + expectedText + " is not equal to actual text " + actualText,
                            "Actual Text should be equal to expected text ",
                            "Actual Text  is not equal to expected text");
                        }
                    }
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed("Verifying Actual Text with Expected Text",
                           "Expected Text or Actual Text is not present",
                           "Actual Text and expected text should Present",
                           "Actual Text or Actual Text is not present");
                }
            }
            return false;
        }

        private static bool VerifyGoogleTag(IWebDriver webDriver, string tagName, string pageName, string googleAnalyticsTag, MasterSheetRow masterSheetRow)
        {
            try
            {
                IList<IWebElement> NodeList = webDriver.FindElements(By.TagName(tagName));
                foreach (IWebElement node in NodeList)
                {
                    string InnerText = node.GetAttribute("innerHTML");
                    if (!string.IsNullOrEmpty(InnerText) && InnerText.Contains(ConfigurationSettings.Get(googleAnalyticsTag, masterSheetRow)))
                    {
                        masterSheetRow.TestResultLogger.Passed(pageName + " > Google Analytics",
                                      "Html indicative of Google tag manager is present in " + tagName + " Tag with identifier as :"
                                      + ConfigurationSettings.Get(googleAnalyticsTag, masterSheetRow),
                                      "Page should contain html indicative of Google tag manager in " + tagName + " Tag",
                                      "Page contains html indicative of Google tag manager in " + tagName + " Tag");
                        return true;
                    }
                }
                return false;
            }
            catch (WebDriverTimeoutException e)
            {
                masterSheetRow.TestResultLogger.Failed(pageName + " > Google Analytics",
                                      "Web Driver timeout exception occurred, while verifying Google tag manager in " + tagName + " Tag",
                                      "Page Source should contain html indicative of Google tag manager in " + tagName + " Tag",
                                      "WebDriverTimeoutException occurred while verifying Google tag manager in " + tagName + " Tag" + e.Message);
                return false;
            }
            catch (ArgumentException e)
            {
                masterSheetRow.TestResultLogger.Failed(pageName + " > Google Analytics",
                                    "ArgumentException xception occurred, while verifying Google tag manager in " + tagName + " Tag",
                                    "Page Source should contain html indicative of Google tag manager in " + tagName + " Tag",
                                    "ArgumentException occured,while verifying Google tag manager in " + tagName + " Tag" + e.Message);
                return false;
            }
            catch (NoSuchElementException e)
            {
                masterSheetRow.TestResultLogger.Passed(pageName + " > Google Analytics",
                                   "NoSuchElement exception occurred, while verifying Google tag manager in " + tagName + " Tag",
                                   "Page Source should contain html indicative of Google tag manager in " + tagName + " Tag",
                                   "NoSuchElementException occured,while verifying Google tag manager in " + tagName + " Tag" + e.Message);
                return false;
            }
            catch (StaleElementReferenceException e)
            {
                masterSheetRow.TestResultLogger.Passed(pageName + " > Google Analytics",
                                     "StaleElementReferenceException exception occurred, while verifying Google tag manager in " + tagName + " Tag",
                                     "Page Source should contain html indicative of Google tag manager in " + tagName + " Tag",
                                     "StaleElementReferenceException occured,while verifying Google tag manager in " + tagName + " Tag" + e.Message);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                masterSheetRow.TestResultLogger.Passed(pageName + " > Google Analytics",
                                         "InvalidOperationException exception occurred, while verifying Google tag manager in " + tagName + " Tag",
                                         "Page Source should contain html indicative of Google tag manager in " + tagName + " Tag",
                                         "InvalidOperationException occured,while verifying Google tag manager in " + tagName + " Tag" + ioe.Message);
                return false;
            }
        }

        public static void VerifyCarousel(this IWebDriver webDriver, string pageName, string pagesWithCarousel, string carouselImagePath, string carouselDotPath
                      , bool carouselDragAndDropEnabled, MasterSheetRow masterSheetRow)
        {
            if (webDriver != null && masterSheetRow != null)
            {
                string carouselConfigKey = ConfigurationSettings.Get(pagesWithCarousel, masterSheetRow, masterSheetRow.TestResultLogger.Country);
                if (carouselConfigKey.Contains(pageName))
                {
                    string carouselDotConfigKey = ConfigurationSettings.Get(carouselDotPath, masterSheetRow, masterSheetRow.TestResultLogger.Country);
                    IList<IWebElement> carouselDots = webDriver.FindElements(carouselDotConfigKey, pageName, "Carousel Dots", masterSheetRow);

                    #region Click On Carousel Dots

                    webDriver.ClickCarouselDot(carouselDots, pageName, carouselImagePath, masterSheetRow);

                    #endregion Click On Carousel Dots

                    if (carouselDots.Count > 1)
                    {
                        webDriver.Navigate().Refresh(); //Refresh Page
                        webDriver.ClickCarouselArrow(carouselImagePath, pageName, masterSheetRow);
                        if (carouselDragAndDropEnabled)
                        {
                            webDriver.Navigate().Refresh(); //Refresh Page
                            webDriver.DragAndDropCarouselImage(carouselImagePath, pageName, masterSheetRow);
                        }
                    }
                }
            }
        }

        public static void ClickCarouselDot(this IWebDriver webDriver, IList<IWebElement> carouselDots, string pageName, string carouselImagePath, MasterSheetRow masterSheetRow)
        {
            if (masterSheetRow != null && carouselDots != null)
            {
                IWebElement carouselImage;
                IWebElement tempImage = null;
                int carouselNumber = 1;
                foreach (IWebElement dot in carouselDots)
                {
                    try
                    {
                        if (dot.IsDisplayed(pageName, "Carousel dot No. : " + carouselNumber, masterSheetRow))
                        {
                            if (carouselNumber == 1)//Incase of First Dot,it is already selected
                            {
                                carouselImage = webDriver.FindElement(ConfigurationSettings.Get(carouselImagePath, masterSheetRow), pageName, "carousel Image", masterSheetRow);
                                if (carouselImage.IsDisplayed(pageName, "Carousel" + carouselNumber + " Image", masterSheetRow))
                                {
                                    tempImage = carouselImage;
                                }
                            }
                            else
                            {
                                dot.Click("Carousel Verification ", "Carousel dot No. :" + carouselNumber, masterSheetRow);
                                //Reinitialize Image element after operation to compare with previous/next image
                                carouselImage = webDriver.FindElement(ConfigurationSettings.Get(carouselImagePath, masterSheetRow), pageName, "carousel Image", masterSheetRow);
                                if (carouselImage.IsDisplayed(pageName, "Carousel" + carouselNumber + " Image", masterSheetRow))
                                {
                                    CompareCarouselImages(tempImage, carouselImage, carouselNumber, masterSheetRow);
                                    tempImage = carouselImage;
                                }
                            }
                        }
                    }
                    catch (NoSuchElementException e)
                    {
                        masterSheetRow.TestResultLogger.Failed("Carousel Selection ",
                                             "NoSuchElement exception occurred, while verifying carousel " + carouselNumber + "in page " + pageName,
                                             "Carousel " + carouselNumber + " Dot should clicked on the page",
                                             "NoSuchElementException occured,while verifying carousel in page" + e.Message);
                    }
                    carouselNumber++;
                }
            }
        }

        public static void CompareCarouselImages(this IWebElement tempImage, IWebElement carouselImage, int carouselNumber, MasterSheetRow masterSheetRow)
        {
            if (tempImage != null && masterSheetRow != null)
            {
                if (!tempImage.Equals(carouselImage))
                {
                    masterSheetRow.TestResultLogger.Passed("Carousel Image", "Carousel : " + carouselNumber + " has different image element",
                                                           "Carousel image element should be different",
                                                           "Carousel image element is different");
                }
                else
                {
                    masterSheetRow.TestResultLogger.Failed("Carousel Image", "Carousel : " + carouselNumber + " image element has same image element",
                                                           "Carousel images element should be different",
                                                           "Carousel has same image element");
                }
            }
        }
    }
}