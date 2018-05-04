using CafeNextFramework.CafeConfiguration;
using CafeNextFramework.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace CafeNextFramework.Reporting
{
    public class ConsolidatedReporter : IDisposable
    {
        private TextWriter fileWriter;
        public string XmlReportsFolderPath { get; set; }
        public string HtmlReportsFolderPath { get; set; }
        public string[] XmlFileNames { get; set; }
        public string ConsolidatedReportXsltFilePath { get; set; }

        public ConsolidatedReporter()
        {
            XmlReportsFolderPath = FileUtilities.GetRunResource("XmlReports");
            XmlFileNames = Directory.GetFiles(XmlReportsFolderPath, "*.xml");
            HtmlReportsFolderPath = FileUtilities.GetRunResource("HtmlEvents");
            ConsolidatedReportXsltFilePath = CafeNextConfiguration.ConsolidatedXsltFilePath;
        }

        public void GenerateConsolidatedReport()
        {
            string marketName = string.Empty;
            string testcaseName = string.Empty;
            string browserName = string.Empty;
            string xmlFilePath = string.Empty;

            XmlDocument xmlDocs = new XmlDocument();
            XmlNode docNode = xmlDocs.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocs.AppendChild(docNode);
            XmlNode MainNode = xmlDocs.CreateElement("ConsolidatedReport");
            xmlDocs.AppendChild(MainNode);

            XmlNode newMarketNode = null, newTestCaseNode = null, newBrowserNode = null;

            foreach (string fileName in XmlFileNames)
            {
                string[] fileNameParts = Path.GetFileName(fileName).Split('_');
                marketName = fileNameParts[0];
                testcaseName = fileNameParts[1];
                browserName = fileNameParts[2].Split('.').First();
                xmlFilePath = Path.GetFullPath(fileName);

                XmlNode marketNode = MainNode.SelectSingleNode("Market[@marketName ='" + marketName + "']");
                if (marketNode == null)
                {
                    newMarketNode = createNewNode(xmlDocs, MainNode, "Market", "marketName", marketName);
                    newTestCaseNode = createNewNode(xmlDocs, newMarketNode, "Testcase", "testcaseName", testcaseName);
                    newBrowserNode = createNewNode(xmlDocs, newTestCaseNode, "Browser", "browserName", browserName);
                    createResultNode(xmlDocs, newBrowserNode, xmlFilePath);
                    createReportNode(xmlDocs, newBrowserNode, fileName);
                }
                else
                {
                    XmlNode testcaseNode = marketNode.SelectSingleNode("Testcase[@testcaseName ='" + testcaseName + "']");
                    if (testcaseNode == null)
                    {
                        newTestCaseNode = createNewNode(xmlDocs, newMarketNode, "Testcase", "testcaseName", testcaseName);
                        newBrowserNode = createNewNode(xmlDocs, newTestCaseNode, "Browser", "browserName", browserName);
                        createResultNode(xmlDocs, newBrowserNode, xmlFilePath);
                        createReportNode(xmlDocs, newBrowserNode, fileName);
                    }
                    else
                    {
                        XmlNodeList browserNodes = testcaseNode.SelectNodes("Browser");
                        if (browserNodes == null)
                        {
                            newBrowserNode = createNewNode(xmlDocs, testcaseNode, "Browser", "browserName", browserName);
                            createResultNode(xmlDocs, newBrowserNode, xmlFilePath);
                            createReportNode(xmlDocs, newBrowserNode, fileName);
                        }
                        else
                        {
                            newBrowserNode = null;
                            foreach (XmlNode node in browserNodes)
                            {
                                if (node.Attributes["browserName"].Value == browserName)
                                {
                                    newBrowserNode = node;
                                    break;
                                }
                            }
                            if (newBrowserNode == null)
                            {
                                newBrowserNode = createNewNode(xmlDocs, testcaseNode, "Browser", "browserName", browserName);
                                createResultNode(xmlDocs, newBrowserNode, xmlFilePath);
                                createReportNode(xmlDocs, newBrowserNode, fileName);
                            }
                        }
                    }
                }
            }
            xmlDocs.Save(XmlReportsFolderPath + "/ConsolidatedReport.xml");

            GenerateHtmlReport();
        }

        public string FetchBrowserResult(string xmlFilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);
            return xmlDoc.SelectSingleNode("//TestCase[@Status=\"2\"]") != null ? "Failed" : "Passed";
        }

        private XmlNode createNewNode(XmlDocument xmlDocs, XmlNode nodeToAppend, string nodeName, string attributeName, string attributeValue)
        {
            XmlNode newNode = xmlDocs.CreateElement(nodeName);
            XmlAttribute attribute = xmlDocs.CreateAttribute(attributeName);
            newNode.Attributes.Append(attribute);
            attribute.Value = attributeValue;
            nodeToAppend.AppendChild(newNode);

            return newNode;
        }

        private void createResultNode(XmlDocument xmlDocs, XmlNode nodeToAppend, string xmlFilePath)
        {
            XmlNode newNode = xmlDocs.CreateElement("Result");
            newNode.InnerText = FetchBrowserResult(xmlFilePath);
            nodeToAppend.AppendChild(newNode);
        }

        private void createReportNode(XmlDocument xmlDocs, XmlNode nodeToAppend, string fileName)
        {
            XmlNode newNode = xmlDocs.CreateElement("HtmlReportPath");
            newNode.InnerText = Path.GetFullPath(string.Concat(HtmlReportsFolderPath, "\\", Path.GetFileName(fileName).Replace(".xml", ".html")));
            nodeToAppend.AppendChild(newNode);
        }

        private void GenerateHtmlReport()
        {
            string xsltFilePath = Path.GetFullPath(ConsolidatedReportXsltFilePath);
            string xmlFilePath = Path.GetFullPath(string.Concat(XmlReportsFolderPath, "/ConsolidatedReport.xml"));

            if (!string.IsNullOrEmpty(xsltFilePath))
            {
                string htmlReportData = TransformXmlToHtml(xmlFilePath, xsltFilePath);
                string htmlFileName = Path.GetFullPath(string.Concat(HtmlReportsFolderPath, "/ConsolidatedReport.html"));
                WriteHtmlData(htmlFileName, htmlReportData);
            }
            else
            {
                Console.WriteLine("XSLT is missing thus HTML report could not be generated.");
            }
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