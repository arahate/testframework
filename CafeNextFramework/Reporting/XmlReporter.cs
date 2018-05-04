using CafeNextFramework.CafeConfiguration;
using CafeNextFramework.Utilities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace CafeNextFramework.Reporting
{
    public class XmlReporter : IReporter, IDisposable
    {
        private readonly XmlDocument xmlDoc = new XmlDocument();
        private TextWriter fileWriter;
        private readonly string screenDumpFolder;
        private string screenDumpFilePath;
        public string XsltFilePath { get; set; }
        public string DetailsPath { get; set; }

        public string HtmlReportPath { get; set; }

        public XmlReporter()
        {
            screenDumpFolder = FileUtilities.GetRunResource("ScreenShots");
            if (!FileUtilities.MakeFolder(screenDumpFolder))
            {
                Console.WriteLine("Cant create/access ScreenShots folder; these will not be generated: " + screenDumpFolder);
            }
            HtmlReportPath = FileUtilities.GetRunResource("HtmlEvents");
            if (!(FileUtilities.MakeFolder(HtmlReportPath)))
            {
                throw new CafeNextFrameworkException("HTMLReporter failed to create folder for storing reports.", new FileNotFoundException());
            }
            Dictionary<string, string> elementAttributes = CafeNextConfiguration.ReporterValues();
            if (elementAttributes.ContainsKey(Constant.XSLTPATH_ATTR))
            {
                XsltFilePath = elementAttributes[Constant.XSLTPATH_ATTR];
            }

            DetailsPath = GetXmlReporterDetailsPath(DetailsPath);
        }

        public string GetXmlReporterDetailsPath(string detailPath)
        {
            if (string.IsNullOrEmpty(detailPath))
            {
                detailPath = FileUtilities.GetRunResource("XmlReports");
            }
            if (!(FileUtilities.MakeFolder(detailPath)))
            {
                throw new CafeNextFrameworkException("Cant create/access xml reports folder; these will not be generated: ", new FileNotFoundException());
            }
            return detailPath;
        }

        public void Log(ResultType resultType, EventData details, string country, string moduleName)
        {
            Report("DETAILS", details, country, moduleName, resultType);
        }

        private void Report(string eventType, EventData eventData, string country, string moduleName, ResultType rsType)
        {
            try
            {
                if (eventType.Equals("DETAILS"))
                {
                    SaveScreenShotImage(country, moduleName, eventData.ScreenShotFile);

                    #region starting

                    if (!VerifyXmlExist(DetailsPath, country, moduleName, eventData))
                    {
                        XmlDocument xmlDocs = new XmlDocument();
                        XmlNode docNode = xmlDocs.CreateXmlDeclaration("1.0", "UTF-8", null);
                        xmlDocs.AppendChild(docNode);
                        XmlNode testCycleNode = xmlDocs.CreateElement("TestCycle");
                        XmlAttribute attribute = xmlDocs.CreateAttribute("Description");
                        testCycleNode.Attributes.Append(attribute);
                        attribute.Value = "Test cycle created by zephyr";
                        attribute = xmlDocs.CreateAttribute("Name");
                        testCycleNode.Attributes.Append(attribute);
                        attribute.Value = country + "_" + moduleName + "_" + DateTime.Now;
                        xmlDocs.AppendChild(testCycleNode);

                        XmlNode testCasesNode = xmlDocs.CreateElement("TestCases");
                        attribute = xmlDocs.CreateAttribute("ProjectId");
                        testCasesNode.Attributes.Append(attribute);
                        attribute.Value = "15000";
                        testCycleNode.AppendChild(testCasesNode);

                        XmlNode testCaseId = xmlDocs.CreateElement("TestCase");
                        attribute = xmlDocs.CreateAttribute("id");
                        testCaseId.Attributes.Append(attribute);
                        attribute.Value = "46852";
                        testCasesNode.AppendChild(testCaseId);

                        XmlNode testStepsNode = xmlDocs.CreateElement("TestSteps");
                        testCaseId.AppendChild(testStepsNode);

                        XmlNode testStepNode = CreateXmlElementWithAttribute(xmlDocs, "TestStep", "stepname", eventData.TestStepDescription);
                        XmlAttribute stepAttribute = (rsType.Equals(ResultType.PASSED)) ?
                            CreateXmlAttribute(xmlDocs, testStepNode, "status", "1") : CreateXmlAttribute(xmlDocs, testStepNode, "status", "2");
                        testStepNode.Attributes.Append(stepAttribute);
                        testStepNode.AppendChild(CreateXmlElement(xmlDocs, "TestData", eventData.TestData));
                        testStepNode.AppendChild(CreateXmlElement(xmlDocs, "Expected", eventData.Expected));
                        testStepNode.AppendChild(CreateXmlElement(xmlDocs, "Actual", eventData.Actual));
                        if (eventData.ScreenShotFile != null)
                        {
                            testStepNode.AppendChild(CreateXmlElement(xmlDocs, "Attachments", "")).AppendChild(
                                                  CreateXmlElementWithAttribute(xmlDocs, "Attachment", "path", ScreenDumpLink(screenDumpFilePath)));
                        }

                        xmlDocs.Save(DetailsPath + "/" + country + "_" + moduleName + "_" + eventData.BrowserName + ".xml");
                    }

                    #endregion starting

                    else
                    {
                        xmlDoc.Load(DetailsPath + "/" + country + "_" + moduleName + "_" + eventData.BrowserName + ".xml");

                        XmlNode root = xmlDoc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0];

                        XmlNode testStepNode = CreateXmlElementWithAttribute(xmlDoc, "TestStep", "stepname", eventData.TestStepDescription);
                        XmlAttribute attribute = (rsType.Equals(ResultType.PASSED)) ?
                           CreateXmlAttribute(xmlDoc, testStepNode, "status", "1") : CreateXmlAttribute(xmlDoc, testStepNode, "status", "2");
                        testStepNode.Attributes.Append(attribute);
                        root.AppendChild(testStepNode);
                        testStepNode.AppendChild(CreateXmlElement(xmlDoc, "TestData", eventData.TestData));
                        testStepNode.AppendChild(CreateXmlElement(xmlDoc, "Expected", eventData.Expected));
                        testStepNode.AppendChild(CreateXmlElement(xmlDoc, "Actual", eventData.Actual));
                        if (eventData.ScreenShotFile != null)
                        {
                            testStepNode.AppendChild(CreateXmlElement(xmlDoc, "Attachments", "")).AppendChild(
                                                  CreateXmlElementWithAttribute(xmlDoc, "Attachment", "path", ScreenDumpLink(screenDumpFilePath)));
                        }

                        xmlDoc.Save(DetailsPath + "/" + country + "_" + moduleName + "_" + eventData.BrowserName + ".xml");

                        GenerateBasicXml(xmlDoc, DetailsPath, country, moduleName, eventData);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("InvalidOperationException Error in XML reporting" + "Country : " + country + "Exception " + e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("ArgumentException Error in XML reporting" + "Country : " + country + "Exception " + e.Message);
            }
            catch (XmlException e)
            {
                Console.WriteLine("XmlException Error in XML reporting" + "Country : " + country + "Exception " + e.Message);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                Console.WriteLine("XPathException Error in XML reporting" + "Country : " + country + "Exception " + e.Message);
            }
            catch (XsltException e)
            {
                Console.WriteLine("XsltException Error in XML reporting" + "Country : " + country + "Exception " + e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException Error in XML reporting" + "Country : " + country + "Exception " + e.Message);
            }
        }

        private void GenerateBasicXml(XmlDocument xmlDocument, string detailsPath, string country, string moduleName, EventData eventData)
        {
            if (xmlDocument != null)
            {
                xmlDocument.Load(detailsPath + "/" + country + "_" + moduleName + "_" + eventData.BrowserName + ".xml");
                XmlNode root = xmlDocument.ChildNodes[1].ChildNodes[0].ChildNodes[0];
                XmlNodeList nodeList = xmlDocument.SelectNodes("//TestStep[@status=\"2\"]");
                if (nodeList.Count <= 0)
                {
                    XmlAttribute attribute = xmlDocument.CreateAttribute("Status");
                    root.Attributes.Append(attribute);
                    attribute.Value = "1";
                }
                else
                {
                    XmlAttribute attribute = xmlDocument.CreateAttribute("Status");
                    root.Attributes.Append(attribute);
                    attribute.Value = "2";
                }
                xmlDocument.Save(detailsPath + "/" + country + "_" + moduleName + "_" + eventData.BrowserName + ".xml");
            }

            GenerateHtmlReport(detailsPath, country, moduleName, eventData);
        }

        private void GenerateHtmlReport(string detailsPath, string country, string moduleName, EventData eventData)
        {
            string xsltFilePath = Path.GetFullPath(XsltFilePath);
            string xmlFilePath = detailsPath + "/" + country + "_" + moduleName + "_" + eventData.BrowserName + ".xml";

            if (!string.IsNullOrEmpty(xsltFilePath))
            {
                string htmlReportData = TransformXmlToHtml(xmlFilePath, xsltFilePath);
                string htmlFileName = HtmlReportPath + "/" + country + "_" + moduleName + "_" + eventData.BrowserName + ".html";
                WriteHtmlData(htmlFileName, htmlReportData);
            }
            else
            {
                Console.WriteLine("XSLT is missing thus HTML report could not be generated.");
            }
        }

        private XmlNode CreateXmlElement(XmlDocument xmlDocs, string elementName, string elementValue)
        {
            XmlNode elementNode = null;
            if (xmlDocs != null)
            {
                elementNode = xmlDocs.CreateElement(elementName);
                if (!string.IsNullOrEmpty(elementValue))
                {
                    elementNode.InnerText = elementValue;
                }
                return elementNode;
            }
            else
            {
                return elementNode;
            }
        }

        private XmlNode CreateXmlElementWithAttribute(XmlDocument xmlDocs, string elementName, string attributeName, string attributeValue)
        {
            XmlNode arrtibuteElementNode = null;
            if (xmlDocs != null)
            {
                arrtibuteElementNode = xmlDocs.CreateElement(elementName);
                XmlAttribute attribute = xmlDocs.CreateAttribute(attributeName);
                arrtibuteElementNode.Attributes.Append(attribute);
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    attribute.Value = attributeValue;
                }
                return arrtibuteElementNode;
            }
            else
            {
                return arrtibuteElementNode;
            }
        }

        private XmlAttribute CreateXmlAttribute(XmlDocument xmlDocs, XmlNode element, string attributeName, string attributeValue)
        {
            XmlAttribute attribute = null;
            if (xmlDocs != null && element != null)
            {
                attribute = xmlDocs.CreateAttribute(attributeName);
                element.Attributes.Append(attribute);
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    attribute.Value = attributeValue;
                }
                return attribute;
            }
            else
            {
                return attribute;
            }
        }

        private string ScreenDumpLink(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return filePath.Replace(@"\", "/");
            }

            return string.Empty;
        }

        private void SaveScreenShotImage(string country, string moduleName, Screenshot screenShotFile)
        {
            if (screenShotFile != null)
            {
                string screenShotName = string.Format("{0}_{1}_{2}.png", country, moduleName, Guid.NewGuid());
                screenDumpFilePath = Path.GetFullPath(screenDumpFolder) + "\\" + screenShotName;
                screenShotFile.SaveAsFile(screenDumpFilePath, ScreenshotImageFormat.Png);
            }
        }

        private bool VerifyXmlExist(string path, string countryName, string userJourneyName, EventData eventData)
        {
            string[] fileNames = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string file in fileNames)
            {
                if ((file.IndexOf(countryName, StringComparison.OrdinalIgnoreCase) >= 0) && (file.IndexOf(userJourneyName, StringComparison.OrdinalIgnoreCase) >= 0)
                    && (file.IndexOf(eventData.BrowserName, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return true;
                }
            }
            return false;
        }

        private string TransformXmlToHtml(string xmlFile, string xsltFile)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(File.OpenRead(xsltFile)))
            {
                transform.Load(reader);
            }
            var results = new StringWriter();
            using (var myFile = File.OpenRead(xmlFile))
            {
                var reader = XmlReader.Create(myFile);
                transform.Transform(reader, null, results);
                myFile.Close();
            }
            return results.ToString();
        }

        private void WriteHtmlData(string filePath, string htmldata)
        {
            OpenHtmlFile(filePath);
            WriteToHtmlFile(htmldata);
            CloseHtmlFile();
        }

        private void OpenHtmlFile(string filePath)
        {
            try
            {
                fileWriter = new StreamWriter(filePath);
            }
            catch (IOException e)
            {
                Console.WriteLine("Error while trying to open a HTML file " + filePath + "Exception " + e.Message);
            }
        }

        private void WriteToHtmlFile(string htmldata)
        {
            try
            {
                fileWriter.WriteLine(htmldata);
            }
            catch (IOException e)
            {
                Console.WriteLine("Error while writing details in HTML File: Exception " + e.Message);
            }
        }

        private void CloseHtmlFile()
        {
            try
            {
                Dispose();
            }
            catch (IOException e)
            {
                Console.WriteLine("Error while closing HTML file: Exception " + e.Message);
            }
            finally
            {
                try
                {
                    if (fileWriter != null)
                    {
                        fileWriter.Close();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error while closing HTML file: Exception " + e.Message);
                }
                finally
                {
                    fileWriter = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && fileWriter != null)
            {
                fileWriter.Dispose();
            }
        }
    }
}