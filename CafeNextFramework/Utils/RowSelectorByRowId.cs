using System.Collections.Generic;

namespace CafeNextFramework.Utilities
{
    public class RowSelectorByRowId : IRowSelector
    {
        private readonly string rowId;

        public RowSelectorByRowId(string rowId)
        {
            this.rowId = rowId;
        }

        public bool CheckRowData(Dictionary<string, string> row)
        {
            if (row != null)
            {
                return string.IsNullOrEmpty(rowId) || rowId.Equals(row["_rowId"]);
            }
            return false;
        }
    }
}