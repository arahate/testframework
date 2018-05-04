using System.Collections.Generic;

namespace CafeNextFramework.Utilities
{
    public interface INamedRowAccess
    {
        Dictionary<string, string> GetRowData();
    }
}