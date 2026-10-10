using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingPolicyTest
    {
        [Fact]
        public void Invalid_policies_rules_and_resolvers_fail_configuration()
        {
            var options = new SharpAbpOperationRateLimitingOptions();
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.AddPolicy("", b => b.WithFixedWindow(TimeSpan.FromSeconds(1), 1)));
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.AddPolicy("empty", b => { }));
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.AddPolicy("negative", b => b.WithFixedWindow(TimeSpan.FromSeconds(1), -1)));
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.AddPolicy("duration", b => b.WithFixedWindow(TimeSpan.Zero, 1)));
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.AddPolicy("duplicate", b => b
                .AddRule(r => r.WithName("same").WithFixedWindow(TimeSpan.FromSeconds(1), 1))
                .AddRule(r => r.WithName("same").WithFixedWindow(TimeSpan.FromSeconds(2), 2))));
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.AddPartitionResolver("Global", (_, _, _) => Task.FromResult("key")));
            options.AddPolicy("unknown", b => b.WithFixedWindow(TimeSpan.FromSeconds(1), 1).PartitionBy("missing"));
            Assert.Throws<OperationRateLimitingConfigurationException>(() => new OperationRateLimitingConfiguration(Options.Create(options)));
        }

        [Fact]
        public void Null_configuration_delegates_and_blank_attribute_names_fail_immediately()
        {
            var options = new SharpAbpOperationRateLimitingOptions();
            Assert.Throws<ArgumentNullException>(() => options.AddPolicy("policy", null!));
            Assert.Throws<ArgumentNullException>(() => options.ConfigurePolicy("policy", null!));
            Assert.Throws<ArgumentNullException>(() => new OperationRateLimitingPolicyBuilder().AddRule(null!));
            Assert.Throws<ArgumentException>(() => new OperationRateLimitingAttribute(" "));
        }

        [Fact]
        public void Replacement_configuration_and_snapshots_preserve_rule_identity()
        {
            var options = new SharpAbpOperationRateLimitingOptions();
            options.AddPolicy("policy", b => b.WithFixedWindow(TimeSpan.FromSeconds(1), 1).PartitionByParameter().WithMultiTenancy());
            Assert.Single(options.Policies["policy"].Rules);
            options.AddPolicy("policy", b => b.AddRule(r => r.WithName("named").WithFixedWindow(TimeSpan.FromMinutes(1), 2))
                .AddRule(r => r.WithName("second").WithFixedWindow(TimeSpan.FromHours(1), 20)));
            options.ConfigurePolicy("policy", b => b.WithErrorCode("custom:code"));
            Assert.Equal(2, options.Policies["policy"].Rules.Count);
            Assert.Equal("custom:code", options.Policies["policy"].ErrorCode);
            var snapshot = new OperationRateLimitingConfiguration(Options.Create(options));
            options.ConfigurePolicy("policy", b => b.ClearRules().WithFixedWindow(TimeSpan.FromDays(1), 100));
            Assert.Equal(2, snapshot.Policies["policy"].Rules.Count);
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.ConfigurePolicy("policy", b => b.ClearRules()));
            options.RemovePolicy("policy");
            Assert.Throws<OperationRateLimitingConfigurationException>(() => options.ConfigurePolicy("policy", b => b.WithErrorCode("x")));
        }

        [Fact]
        public void Stable_length_prefixed_keys_distinguish_component_boundaries_and_rule_versions()
        {
            Assert.NotEqual(OperationRateLimitingKeyBuilder.HashComponents("ab", "c"), OperationRateLimitingKeyBuilder.HashComponents("a", "bc"));
            Assert.NotEqual(OperationRateLimitingKeyBuilder.HashComponents("A"), OperationRateLimitingKeyBuilder.HashComponents("a"));
            var keys = new OperationRateLimitingKeyBuilder();
            var named = new OperationRateLimitingRuleDefinition("named", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromSeconds(1), 1);
            var changed = new OperationRateLimitingRuleDefinition("named", typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromHours(1), 100);
            Assert.Equal(keys.Build("app", "policy", named, "key", "host"), keys.Build("app", "policy", changed, "key", "host"));
            Assert.NotEqual(keys.Build("app", "policy", named, "key", "host"), keys.Build("another", "policy", named, "key", "host"));
            var unnamed = new OperationRateLimitingRuleDefinition(null, typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromSeconds(1), 1);
            var unnamedChanged = new OperationRateLimitingRuleDefinition(null, typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromSeconds(1), 2);
            Assert.NotEqual(unnamed.Name, unnamedChanged.Name);
            var legacyPhone = new OperationRateLimitingRuleDefinition(null,
                typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromSeconds(1), 1, "PhoneNumber");
            var compositeApiSinglePhone = new OperationRateLimitingRuleDefinition(null,
                typeof(FixedWindowOperationRateLimitingRule), TimeSpan.FromSeconds(1), 1,
                new[] { OperationRateLimitingPartition.PhoneNumber() });
            Assert.Equal(legacyPhone.Name, compositeApiSinglePhone.Name);
        }

        [Fact]
        public void Final_snapshot_rejects_reserved_resolvers_even_when_dictionary_is_modified_directly()
        {
            var options = new SharpAbpOperationRateLimitingOptions();
            options.PartitionResolvers["Global"] = (_, _, _) => Task.FromResult("override");
            Assert.Throws<OperationRateLimitingConfigurationException>(() => new OperationRateLimitingConfiguration(Options.Create(options)));
        }

        [Fact]
        public async Task Same_name_resolver_registration_uses_last_definition()
        {
            var options = new SharpAbpOperationRateLimitingOptions();
            options.AddPartitionResolver("Account", (_, _, _) => Task.FromResult("old"));
            options.AddPartitionResolver("Account", (_, _, _) => Task.FromResult("new"));
            var snapshot = new OperationRateLimitingConfiguration(Options.Create(options));
            Assert.Single(snapshot.PartitionResolvers);
            Assert.Equal("new", await snapshot.PartitionResolvers["Account"](null!, new OperationRateLimitingContext(), default));
        }

        [Fact]
        public void Named_arguments_reject_duplicates_and_preserve_types()
        {
            var arguments = new OperationRateLimitingArguments()
                .Add("CustomerId", Guid.Empty);

            Assert.Equal(Guid.Empty, arguments.GetValue<Guid>("CustomerId"));
            Assert.Throws<OperationRateLimitingContextException>(() => arguments.Add("CustomerId", "other"));
            Assert.Throws<OperationRateLimitingContextException>(() => arguments.GetValue<string>("CustomerId"));
        }
    }
}
