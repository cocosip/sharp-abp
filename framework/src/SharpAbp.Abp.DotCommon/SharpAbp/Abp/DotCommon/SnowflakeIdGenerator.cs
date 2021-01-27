using DotCommon.Utility;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.DotCommon
{
    public class SnowflakeIdGenerator : ISnowflakeIdGenerator, ITransientDependency
    {
        protected SnowflakeDistributeId SnowflakeDistributeId { get; }

        public SnowflakeIdGenerator(IOptions<SnowflakeIdOptions> options)
        {
            SnowflakeDistributeId = new SnowflakeDistributeId(options.Value.WorkerId, options.Value.DatacenterId);
        }

        public virtual long Create()
        {
            return SnowflakeDistributeId.NextId();
        }
    }
}
