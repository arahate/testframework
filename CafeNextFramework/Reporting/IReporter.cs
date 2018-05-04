namespace CafeNextFramework.Reporting
{
    public interface IReporter
    {
        void Log(ResultType resultType, EventData details, string country, string moduleName);
    }
}