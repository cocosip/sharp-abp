using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpAbp.Abp.Faster
{
    public class AbpFasterOptions
    {
        /// <summary>
        /// 文件存储的根目录
        /// </summary>
        public string? RootPath { get; set; }
        public AbpFasterConfigurations Configurations { get; }
        public AbpFasterOptions()
        {
            Configurations = new AbpFasterConfigurations();
        }

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
