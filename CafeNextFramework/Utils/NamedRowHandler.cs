using System.Linq;

namespace CafeNextFramework.Utilities
{
    public abstract class NamedRowHandler : IRowHandler
    {
        private string[] colNames;

        public abstract bool HandleRow(INamedRowAccess namedRowAccess);

        public bool HandleRow(IRowAccess rowAccess, int rowIx)
        {
            string[] colValues = null;

            if (rowAccess != null)
            {
                colValues = rowAccess.GetColumnValue();
            }
            if (rowIx == 0)
            { // header row
                colNames = colValues.Where(val => val != null).ToArray();
                return true;
            }
            return HandleRow(new RowAccess(rowAccess, colNames)); //To add row from excel file
        }
    }
}