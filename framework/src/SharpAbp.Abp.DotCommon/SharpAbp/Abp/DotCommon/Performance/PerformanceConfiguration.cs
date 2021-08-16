using Volo.Abp.Collections;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class PerformanceConfiguration
    {
        public int StatIntervalSeconds { get; set; } = 1;
        public bool AutoLogging { get; set; } = true;

        public ITypeList<ILogContextTextService> LogContextTexts { get; set; }
        public ITypeList<IPerformanceInfoHandlerService> PerformanceInfoHandlers { get; set; }

        public PerformanceConfiguration()
        {
            LogContextTexts = new TypeList<ILogContextTextService>();
            PerformanceInfoHandlers = new TypeList<IPerformanceInfoHandlerService>();
        }
    }
}
