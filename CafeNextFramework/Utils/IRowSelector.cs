using System.Collections.Generic;

namespace CafeNextFramework.Utilities
{
    public interface IRowSelector
    {
        bool CheckRowData(Dictionary<string, string> row);
    }
}