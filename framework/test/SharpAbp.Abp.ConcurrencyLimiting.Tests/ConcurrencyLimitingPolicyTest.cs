using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


namespace SharpAbp.Abp.ConcurrencyLimiting
{

    public class ConcurrencyLimitingPolicyTest
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 0)]
        [InlineData(1, -1)]
        public void Invalid_capacity_is_rejected(int permits, int queue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SharpAbpConcurrencyLimitingOptions()
                .AddPolicy("p", b => b.WithConcurrency(permits, queue)));
        }

        [Fact]
        public void Invalid_configuration_is_rejected()
        {
            var options = new SharpAbpConcurrencyLimitingOptions();
            Assert.Throws<ArgumentException>(() => options.AddPolicy(" ", b => b.WithConcurrency(1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => options.AddPolicy("p", b => b.WithConcurrency(1).WithQueueTimeout(TimeSpan.Zero)));
            Assert.Throws<ArgumentOutOfRangeException>(() => options.AddPolicy("p", b => b.WithConcurrency(1).WithQueueTimeout(TimeSpan.FromSeconds(-1))));
            Assert.Throws<ConcurrencyLimitingConfigurationException>(() => options.ConfigurePolicy("unknown", b => b.WithConcurrency(2)));
        }

        [Fact]
        public void Replacement_partial_update_and_immutable_snapshot()
        {
            var options = new SharpAbpConcurrencyLimitingOptions();
            ConcurrencyLimitingPolicyBuilder? retained = null;
            options.AddPolicy("p", b => { retained = b; b.WithConcurrency(2, 3).PartitionByParameter().WithQueueTimeout(TimeSpan.FromSeconds(7)); });
            var provider = new OptionsConcurrencyLimitingPolicyProvider(Options.Create(options));
            retained!.WithConcurrency(99);
            Assert.Equal(2, provider.Get("p").PermitLimit);
            options.ConfigurePolicy("p", b => b.WithErrorCode("Custom:Limit"));
            var updated = new OptionsConcurrencyLimitingPolicyProvider(Options.Create(options)).Get("p");
            Assert.Equal(3, updated.QueueLimit);
            Assert.Equal(TimeSpan.FromSeconds(7), updated.QueueTimeout);
            Assert.Equal("Custom:Limit", updated.ErrorCode);
            options.ConfigurePolicy("p", builder => builder.WithQueueTimeout(TimeSpan.FromSeconds(12)));
            var twiceConfigured = new OptionsConcurrencyLimitingPolicyProvider(Options.Create(options)).Get("p");
            Assert.Equal(2, twiceConfigured.PermitLimit);
            Assert.Equal(3, twiceConfigured.QueueLimit);
            Assert.Equal("Custom:Limit", twiceConfigured.ErrorCode);
            Assert.Equal(TimeSpan.FromSeconds(12), twiceConfigured.QueueTimeout);
            options.AddPolicy("p", b => b.WithConcurrency(5));
            Assert.Equal(5, new OptionsConcurrencyLimitingPolicyProvider(Options.Create(options)).Get("p").PermitLimit);
            Assert.Equal(2, provider.Get("p").PermitLimit);
            Assert.Throws<ConcurrencyLimitingConfigurationException>(() => provider.Get("absent"));
        }

        [Fact]
        public void Resolver_add_is_last_wins_and_explicit_replacement_requires_existing_name()
        {
            var options = new SharpAbpConcurrencyLimitingOptions().AddPartitionKeyResolver("r", (_, _) => "first");
            Assert.Same(options, options.AddPartitionKeyResolver("r", (_, _) => "other"));
            options.AddPolicy("p", builder => builder.WithConcurrency(1).PartitionBy("r"));
            using (var services = TestServices.Create(options))
            {
                Assert.Equal("other", services.GetRequiredService<IConcurrencyLimitingPartitionResolver>().Resolve(
                    services.GetRequiredService<IConcurrencyLimitingPolicyProvider>().Get("p"), new ConcurrencyLimitingContext()).Value);
            }
            options.ReplacePartitionKeyResolver("r", (_, _) => "replacement");
            Assert.Throws<ConcurrencyLimitingConfigurationException>(() => options.ReplacePartitionKeyResolver("absent", (_, _) => "x"));
        }
    }

}
