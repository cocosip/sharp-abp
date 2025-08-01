using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.Snowflakes
{
    /// <summary>
    /// Options class for configuring Snowflake ID generation.
    /// This class holds configurations for various Snowflake instances.
    /// </summary>
    public class AbpSnowflakesOptions
    {
        /// <summary>
        /// Gets the collection of Snowflake configurations.
        /// </summary>
        public SnowflakeConfigurations Snowflakes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpSnowflakesOptions"/> class.
        /// </summary>
        public AbpSnowflakesOptions()
        {
            Snowflakes = new SnowflakeConfigurations();
        }

        /// <summary>
        /// Configures the Snowflake options from an <see cref="IConfiguration"/> instance.
        /// It reads the "SnowflakeOptions" section to load multiple Snowflake configurations.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The current <see cref="AbpSnowflakesOptions"/> instance.</returns>
        public AbpSnowflakesOptions Configure(IConfiguration configuration)
        {
            var snowflakeConfigurations = configuration
                .GetSection("SnowflakeOptions")
                .Get<Dictionary<string, SnowflakeConfiguration>>();

            if (snowflakeConfigurations != null)
            {
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
            }
            return this;
        }
    }
}
