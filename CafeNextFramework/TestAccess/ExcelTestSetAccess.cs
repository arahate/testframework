using CafeNextFramework.CafeConfiguration;
using CafeNextFramework.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CafeNextFramework.TestAccess
{
    public class ExcelTestSetAccess
    {
        private readonly List<MasterSheetRow> _masterSheetRows = new List<MasterSheetRow>();

        public ExcelTestSetAccess()
        {
            string testCaseName;
            string browserName;
            List<Dictionary<string, string>> testInstances;
            string excelFilePath = CafeNextConfiguration.ExcelFilePath;
            string excelSheetName = CafeNextConfiguration.ExcelSheetName;

            if (!string.IsNullOrEmpty(excelFilePath) && !string.IsNullOrEmpty(excelSheetName))
            {
                DTAccess dtAccess = new DTAccess(Path.GetFullPath(excelFilePath));
                var sheetName = Path.GetFileName(excelSheetName);
                testInstances = dtAccess.ReadSheet(sheetName);
                int index = 0;
                for (int ix = 0; ix < testInstances.Count; ix++)
                {
                    try
                    {
                        if (!StringUtilities.Match((testInstances[ix]["Execute"]), StringUtilities.Pattern("(Pass|True|Yes|Y|true)", true)))
                        {
                            continue;
                        }
                        testCaseName = testInstances[ix]["AppName"];
                        browserName = testInstances[ix]["BrowserName"];

                        MasterSheetRow row = new MasterSheetRow() { TestCaseName = testCaseName, BrowserName = browserName };
                        _masterSheetRows.Add(row);
                        index++;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("No input value is provided for Execute column" + e.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("ExcelSheet Path or Sheet Name is Not Provided");
            }
        }

        public List<MasterSheetRow> Scripts
        {
            get { return _masterSheetRows; }
        }
    }
}