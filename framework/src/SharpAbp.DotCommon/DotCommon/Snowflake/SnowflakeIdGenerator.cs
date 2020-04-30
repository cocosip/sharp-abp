using DotCommon.Utility;
using Microsoft.Extensions.Options;

namespace SharpAbp.DotCommon.Snowflake
{
    public class SnowflakeIdGenerator
    {
        private readonly SnowflakeIdGeneratorOption _option;
        private readonly SnowflakeDistributeId _snowflakeDistributeId;

        /// <summary>Ctor
        /// </summary>
        public SnowflakeIdGenerator(IOptions<SnowflakeIdGeneratorOption> option)
        {
            _option = option.Value;
            _snowflakeDistributeId = new SnowflakeDistributeId(_option.WorkerId, _option.DataCenterId);
        }

        /// <summary>生成Id
        /// </summary>
        public long GenerateId()
        {
            return _snowflakeDistributeId.NextId();
        }

        /// <summary>生成string类型的Id
        /// </summary>
        public string GenerateIdAsString()
        {
            return _snowflakeDistributeId.NextId().ToString();
        }
    }
}
