using System.IO;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SharpAbp.Abp.Faster
{
    [DependsOn(
        typeof(AbpFasterModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
        )]
    public class AbpFasterTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpFasterOptions>(options =>
            {
                options.RootPath = Path.Combine(Path.GetTempPath(), "sharp-abp-faster-tests");

                options.Configurations.Configure<FasterTestEntry>(c =>
                {
                    c.FileName = "test-entry.log";
                    c.PreallocateFile = false;
                    c.CommitIntervalMillis = 100;
                    c.CompleteIntervalMillis = 200;
                    c.TruncateIntervalMillis = 600000;
                });

                options.Configurations.Configure("shared-cache-key", c =>
                {
                    c.FileName = "shared-cache-key.log";
                    c.PreallocateFile = false;
                    c.CommitIntervalMillis = 100;
                    c.CompleteIntervalMillis = 200;
                    c.TruncateIntervalMillis = 600000;
                });

                options.Configurations.Configure<FasterConcurrentTestEntry>(c =>
                {
                    c.FileName = "concurrent-test-entry.log";
                    c.PreallocateFile = false;
                    c.CommitIntervalMillis = 100;
                    c.CompleteIntervalMillis = 200;
                    c.TruncateIntervalMillis = 600000;
                });

                options.Configurations.Configure<FasterLateCommitTestEntry>(c =>
                {
                    c.FileName = "late-commit-test-entry.log";
                    c.PreallocateFile = false;
                    c.CommitIntervalMillis = 100;
                    c.CompleteIntervalMillis = 200;
                    c.TruncateIntervalMillis = 600000;
                });
            });
        }
    }
}
