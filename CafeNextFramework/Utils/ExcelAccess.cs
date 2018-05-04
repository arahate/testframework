using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace CafeNextFramework.Utilities
{
    public static class ExcelAccess
    {
        public static int AccessSheet(string fileName, string sheetName, IRowHandler rowHandler)
        {
            ISheet sheet = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                string fileExtension = Path.GetExtension(fileName);
                using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        XSSFWorkbook excelFile = new XSSFWorkbook(file);
                        file.Close();
                        sheet = excelFile.GetSheet(sheetName);
                    }
                    else
                    {
                        HSSFWorkbook excelFile = new HSSFWorkbook(file);
                        file.Close();
                        sheet = excelFile.GetSheet(sheetName);
                    }
                }
            }
            int accessCount = AccessRowCount(sheet, rowHandler);

            return accessCount;
        }

        public static int AccessRowCount(ISheet sheet, IRowHandler rowHandler)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException();
            }

            int rowcount = 0;
            int count = 1;

            int lastCellNumber = sheet.GetRow(0).LastCellNum;
            if (sheet != null)
            {
                rowcount = sheet.LastRowNum;
            }
            if (rowcount == 0 && sheet.GetRow(0) == null)
            {
                return 0;
            }
            SetRowHandler(sheet, rowcount, lastCellNumber, count, rowHandler);
            //Appending row count with 2 as we want to include 0th row and last row
            count = rowcount + 2;
            return count;
        }

        public static void SetRowHandler(ISheet sheet, int rowCount, int lastCellNumber, int count, IRowHandler rowHandler)
        {
            List<string> cellList = new List<string>();
            int index = 0;
            bool rowCounter = false;

            if (sheet != null)
            {
                int row1 = 0;
                do
                {
                    for (int col1 = 0; col1 < lastCellNumber; col1++)
                    {
                        string value = (sheet.GetRow(row1).GetCell(col1) != null) ? sheet.GetRow(row1).GetCell(col1).ToString() : string.Empty;
                        cellList.Add(value);
                    }
                    if (rowHandler != null)
                    {
                        rowCounter = rowHandler.HandleRow(new SimpleRowAccess(cellList), index);
                    }
                    ++count;
                    cellList.Clear();
                    index++;
                    row1++;
                } while (row1 <= rowCount && rowCounter);
            }
        }
    }
}