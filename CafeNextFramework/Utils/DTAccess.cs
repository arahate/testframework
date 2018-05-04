using System.Collections.Generic;

namespace CafeNextFramework.Utilities
{
    public class DTAccess
    {
        private readonly string filePath;

        public DTAccess(string excelFilePath)
        {
            this.filePath = excelFilePath;
        }

        public List<Dictionary<string, string>> ReadSheet(string sheetName)
        {
            return ReadSelectedRows(sheetName, string.Empty);
        }

        public List<Dictionary<string, string>> ReadSelectedRows(string sheetName, IRowSelector selector)

        {
            MyHandler mh = new MyHandler(selector);
            ExcelAccess.AccessSheet(filePath, sheetName, mh);
            return mh.Rows;
        }

        public List<Dictionary<string, string>> ReadSelectedRows(string sheetName, string rowId)
        {
            IRowSelector selector = new RowSelectorByRowId(rowId);
            List<Dictionary<string, string>> selectedRows;
            selectedRows = ReadSelectedRows(sheetName, selector);
            return selectedRows;
        }
    }
}