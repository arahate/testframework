using System.Collections.Generic;

namespace CafeNextFramework.Utilities
{
    public class RowAccess : INamedRowAccess
    {
        private readonly IRowAccess rowAccess;
        private readonly string[] colNames;

        public RowAccess(IRowAccess rowAccess, string[] colNames)
        {
            this.rowAccess = rowAccess;
            this.colNames = colNames;
        }

        public Dictionary<string, string> GetRowData()
        {
            string[] values = rowAccess.GetColumnValue();
            Dictionary<string, string> namedValues = new Dictionary<string, string>();
            for (int ix = 0; ix < values.Length && ix < colNames.Length; ix++)
            {
                string colName = colNames[ix];
                namedValues.Add(colName, values[ix]);
            }
            return new Dictionary<string, string>(namedValues);
        }
    }
}