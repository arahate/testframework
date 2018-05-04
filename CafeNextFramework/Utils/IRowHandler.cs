namespace CafeNextFramework.Utilities
{
    public interface IRowHandler
    {
        bool HandleRow(IRowAccess rowAccess, int rowIx);
    }
}