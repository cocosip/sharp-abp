using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.Faster
{
    /// <summary>
    /// Options for configuring the Faster storage system.
    /// </summary>
    public class AbpFasterOptions
    {
        /// <summary>
        /// Gets or sets the root path for file storage.
        /// </summary>
        public string? RootPath { get; set; }
        
        /// <summary>
        /// Gets the collection of Faster configurations.
        /// </summary>
        public AbpFasterConfigurations Configurations { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AbpFasterOptions"/> class.
        /// </summary>
        public AbpFasterOptions()
        {
            Configurations = new AbpFasterConfigurations();
        }

        /// <summary>
        /// Configures the Faster options from the provided configuration section.
        /// </summary>
        /// <param name="configuration">The configuration containing Faster settings.</param>
        /// <returns>The current instance for method chaining.</returns>
        public AbpFasterOptions Configure(IConfiguration configuration)
        {
            var fasterConfigurations = configuration
                .GetSection("FasterOptions:Configurations")
                .Get<Dictionary<string, AbpFasterConfiguration>>();

            RootPath = configuration["FasterOptions:RootPath"];

            if (fasterConfigurations != null)
            {
                foreach (var kv in fasterConfigurations)
                {
                    Configurations.Configure(kv.Key, c =>
                    {
                        var val = kv.Value;
                        c.FileName = val.FileName;
                        c.PreallocateFile = val.PreallocateFile;
                        c.Capacity = val.Capacity;
                        c.RecoverDevice = val.RecoverDevice;
                        c.UseIoCompletionPort = val.UseIoCompletionPort;
                        c.DisableFileBuffering = val.DisableFileBuffering;
                        c.ScanUncommitted = val.ScanUncommitted;
                        c.AutoRefreshSafeTailAddress = val.AutoRefreshSafeTailAddress;
                        c.CommitIntervalMillis = val.CommitIntervalMillis;
                        c.CompleteIntervalMillis = val.CompleteIntervalMillis;
                        c.TruncateIntervalMillis = val.TruncateIntervalMillis;
                        c.PreReadCapacity = val.PreReadCapacity;
                        c.MaxCommitSkip = val.MaxCommitSkip;
                        c.IteratorName = val.IteratorName;
                    });
                }
            }

            return this;
        }
    }
}