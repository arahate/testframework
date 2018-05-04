using System.Collections.Generic;

namespace CafeNextFramework.Utilities
{
    public class SimpleRowAccess : IRowAccess
    {
        private readonly List<string> cells;

        public SimpleRowAccess(List<string> cells)
        {
            this.cells = cells;
        }

        public string[] GetColumnValue()
        {
            string[] colValues = cells.ToArray();
            return colValues;
        }
    }
}