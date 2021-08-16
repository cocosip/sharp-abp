namespace SharpAbp.Abp.DotCommon.Performance
{
    public interface IPerformanceInfoHandlerService
    {
        void Handle(string name, string key, PerformanceInfo performanceInfo);
    }
}
