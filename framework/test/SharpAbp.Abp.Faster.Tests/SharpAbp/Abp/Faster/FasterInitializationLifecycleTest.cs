using System;
using System.IO;
using FASTER.core;
using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.Faster;

public class FasterInitializationLifecycleTest : AbpFasterTestBase
{
    [Fact]
    public void Failed_Configure_Releases_The_Opened_Device_And_Allows_Retry()
    {
        var root = Path.Combine(Path.GetTempPath(), "sharp-abp-faster-init-" + Guid.NewGuid().ToString("N"));
        var options = GetRequiredService<IOptions<AbpFasterOptions>>().Value;
        options.RootPath = root;
        var factory = GetRequiredService<IFasterLoggerFactory>();
        IDevice device = null;
        var failure = new InvalidOperationException("configuration failed after opening device");
        options.Configurations.Configure("partial-init", configuration =>
        {
            configuration.FileName = "partial.log";
            configuration.DisableFileBuffering = false;
            configuration.Configure = settings =>
            {
                device = settings.LogDevice;
                using var log = new FasterLog(settings);
                log.Enqueue(new byte[] { 1 });
                log.Commit(true);
                throw failure;
            };
        });

        try
        {
            Assert.Same(failure, Assert.Throws<InvalidOperationException>(
                () => factory.GetOrCreate<FasterTestEntry>("partial-init")));
            Assert.NotNull(device);
            var files = Directory.GetFiles(root, "*.0", SearchOption.AllDirectories);
            Assert.NotEmpty(files);
            foreach (var file in files)
            {
                using var exclusive = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }

            options.Configurations.Configure("partial-init", configuration => configuration.Configure = null);
            Assert.True(factory.GetOrCreate<FasterTestEntry>("partial-init").Initialized);
        }
        finally
        {
            (factory as IDisposable)?.Dispose();
            device?.Dispose();
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }
}
