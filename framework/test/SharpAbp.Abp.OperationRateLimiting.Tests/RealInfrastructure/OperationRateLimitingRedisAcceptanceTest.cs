using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;
using Xunit.Abstractions;

namespace SharpAbp.Abp.OperationRateLimiting.RealInfrastructure
{
    public class RedisAcceptanceFactAttribute : FactAttribute
    {
        public RedisAcceptanceFactAttribute()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SHARP_ABP_OPERATION_REDIS")))
            {
                Skip = "SHARP_ABP_OPERATION_REDIS is not configured; real two-process distributed acceptance is unverified.";
            }
        }
    }

    public class OperationRateLimitingRedisAcceptanceTest
    {
        private readonly ITestOutputHelper _output;

        public OperationRateLimitingRedisAcceptanceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Worker_reports_missing_configuration_as_infrastructure_failure()
        {
            var start = new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            start.ArgumentList.Add(typeof(Program).Assembly.Location);
            start.ArgumentList.Add("--operation-worker");
            start.ArgumentList.Add("reset");
            start.ArgumentList.Add("unused");
            start.Environment.Remove("SHARP_ABP_OPERATION_REDIS");
            start.Environment.Remove("SHARP_ABP_OPERATION_ACCEPTANCE_NAMESPACE");
            using (var process = Process.Start(start)!)
            {
                var output = process.StandardOutput.ReadToEndAsync();
                var errors = process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
                var result = await output;
                await errors;
                Assert.Equal(2, process.ExitCode);
                Assert.Contains("INFRASTRUCTURE_FAILURE:", result);
                Assert.DoesNotContain("RESULT:", result);
                _output.WriteLine("Worker infrastructure failure exit=2; it did not report quota exhaustion or admission.");
            }
        }

        [RedisAcceptanceFact]
        public async Task Two_processes_share_redis_cache_lock_admission_and_reset()
        {
            var testNamespace = "sharp-abp-operation-acceptance-" + Guid.NewGuid().ToString("N");
            using (var connection = await ConnectionMultiplexer.ConnectAsync(Environment.GetEnvironmentVariable("SHARP_ABP_OPERATION_REDIS")!))
            {
                var database = connection.GetDatabase();
                try
                {
                    await RunWorkerAsync(testNamespace, "reset", "key");
                    var first = RunWorkerAsync(testNamespace, "consume", "key");
                    var second = RunWorkerAsync(testNamespace, "consume", "key");
                    var deadline = DateTime.UtcNow.AddSeconds(30);
                    while ((int)(await database.StringGetAsync(testNamespace + ":ready")) < 2)
                    {
                        if (DateTime.UtcNow >= deadline)
                        {
                            throw new TimeoutException("Two acceptance workers did not become ready.");
                        }
                        await Task.Delay(25);
                    }
                    await database.StringSetAsync(testNamespace + ":gate", "go", TimeSpan.FromMinutes(2));
                    var totals = await Task.WhenAll(first, second);
                    Assert.Equal(7, totals.Sum());
                    _output.WriteLine("Two processes accepted {0}+{1}={2} calls; expected 7.", totals[0], totals[1], totals.Sum());
                    await RunWorkerAsync(testNamespace, "reset", "key");
                    Assert.Equal(7, await RunWorkerAsync(testNamespace, "consume", "key"));
                    _output.WriteLine("Reset restored 7 admissions; no infrastructure failure was reported.");
                    await RunWorkerAsync(testNamespace, "reset", "key");
                }
                finally
                {
                    await database.KeyDeleteAsync(new RedisKey[] { testNamespace + ":ready", testNamespace + ":gate" });
                }
            }
        }

        private async Task<int> RunWorkerAsync(string testNamespace, string mode, string key)
        {
            var start = new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            start.ArgumentList.Add(typeof(Program).Assembly.Location);
            start.ArgumentList.Add("--operation-worker");
            start.ArgumentList.Add(mode);
            start.ArgumentList.Add(key);
            start.Environment["SHARP_ABP_OPERATION_ACCEPTANCE_NAMESPACE"] = testNamespace;
            using (var process = Process.Start(start)!)
            {
                var output = process.StandardOutput.ReadToEndAsync();
                var errors = process.StandardError.ReadToEndAsync();
                try
                {
                    await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(60));
                }
                catch
                {
                    process.Kill(entireProcessTree: true);
                    throw;
                }
                var result = await output;
                await errors;
                // Never echo arbitrary provider output, which may include credentials.
                var failure = result.Split('\n').FirstOrDefault(line => line.StartsWith("INFRASTRUCTURE_FAILURE:", StringComparison.Ordinal));
                if (failure != null)
                {
                    _output.WriteLine(failure.Trim());
                }
                Assert.Equal(0, process.ExitCode);
                var line = result.Split('\n').Single(value => value.StartsWith("RESULT:", StringComparison.Ordinal)).Trim();
                return mode == "reset" ? 0 : int.Parse(line.Substring("RESULT:".Length), System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
