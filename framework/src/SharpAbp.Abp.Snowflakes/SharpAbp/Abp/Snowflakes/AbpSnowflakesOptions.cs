using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.Snowflakes
{
    public class AbpSnowflakesOptions
    {
        public SnowflakeConfigurations Snowflakes { get; }

        public AbpSnowflakesOptions()
        {
            Snowflakes = new SnowflakeConfigurations();
        }


        public AbpSnowflakesOptions Configure(IConfiguration configuration)
        {
            var snowflakeConfigurations = configuration.Get<Dictionary<string, SnowflakeConfiguration>>();

            foreach (var keyValuePair in snowflakeConfigurations)
            {
                var snowflakeConfiguration = keyValuePair.Value;

                Snowflakes.Configure(keyValuePair.Key, c =>
                {
                    c.Twepoch = snowflakeConfiguration.Twepoch;
                    c.WorkerIdBits = snowflakeConfiguration.WorkerIdBits;
                    c.DatacenterIdBits = snowflakeConfiguration.DatacenterIdBits;
                    c.SequenceBits = snowflakeConfiguration.SequenceBits;
                    c.WorkerId = snowflakeConfiguration.WorkerId;
                    c.DatacenterId = snowflakeConfiguration.DatacenterId;
                });
            }

            return this;
        }
    }
}
