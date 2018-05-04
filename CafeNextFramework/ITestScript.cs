using CafeNextFramework.TestAccess;

namespace CafeNextFramework
{
    public interface ITestScript
    {
        void ExecuteScript(MasterSheetRow masterSheetRow);
    }
}