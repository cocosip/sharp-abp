using SharpAbp.Abp.DotCommon.Performance;
using Volo.Abp.DependencyInjection;

namespace PerformanceSample
{
    public class Service1LogContextTextService : ILogContextTextService, ITransientDependency
    {
        public string GetLogContextText(string name, string key)
        {
            return "Service1";
        }
    }
}
