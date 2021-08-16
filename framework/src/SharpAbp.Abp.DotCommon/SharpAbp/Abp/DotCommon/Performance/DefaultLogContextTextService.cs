using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DotCommon.Performance
{
    public class DefaultLogContextTextService : ILogContextTextService, ITransientDependency
    {
        public DefaultLogContextTextService()
        {

        }

        public virtual string GetLogContextText(string name, string key)
        {
            return $"{name}-{key} logging";
        }
    }
}
