using System.Collections.Generic;

namespace CafeNextFramework.Utilities
{
    public class MyHandler : NamedRowHandler
    {
        public List<Dictionary<string, string>> Rows { get; set; } = new List<Dictionary<string, string>>();

        private readonly IRowSelector selector;

        public MyHandler(IRowSelector sa)
        {
            selector = sa;
        }

        public override bool HandleRow(INamedRowAccess namedRowAccess)
        {
            if (namedRowAccess == null)
            {
                throw new CafeNextFrameworkException();
            }

            if (selector == null || selector.CheckRowData(namedRowAccess.GetRowData()))
            {
                Rows.Add(namedRowAccess.GetRowData());
            }
            return true;
        }
    }
}