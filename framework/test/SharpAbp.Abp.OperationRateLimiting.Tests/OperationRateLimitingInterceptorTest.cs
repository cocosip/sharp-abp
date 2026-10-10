using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.OperationRateLimiting.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Xunit;

namespace SharpAbp.Abp.OperationRateLimiting
{
    public class OperationRateLimitingInterceptorTest : SharpAbpOperationRateLimitingTestBase
    {
        [Fact]
        public async Task Marker_has_priority_over_dtos_and_dto_ambiguity_prevents_execution()
        {
            var service = GetRequiredService<ParameterOperations>();
            await service.Marked("explicit", new KeyDto("dto1"), new KeyDto("dto2"));
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => service.Marked("explicit", new KeyDto("unused1"), new KeyDto("unused2")));
            await service.Dto(new KeyDto("dto1"));
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => service.Dto(new KeyDto("dto1")));
            await Assert.ThrowsAsync<OperationRateLimitingConfigurationException>(() => service.Ambiguous(new KeyDto("a"), new KeyDto("b")));
            await Assert.ThrowsAsync<OperationRateLimitingConfigurationException>(() => service.Markers("a", "b"));
            await Assert.ThrowsAsync<OperationRateLimitingContextException>(() => service.Dto(new KeyDto(null)));
            await Assert.ThrowsAsync<OperationRateLimitingConfigurationException>(() => service.InvalidMarker(42));
        }

        [Fact]
        public async Task Unique_token_and_async_method_requirements_are_explicit()
        {
            var service = GetRequiredService<ParameterOperations>();
            await Assert.ThrowsAsync<OperationRateLimitingConfigurationException>(() => service.Tokens(default, default));
            Assert.Throws<OperationRateLimitingConfigurationException>(() => service.Sync());
            await Assert.ThrowsAsync<OperationRateLimitingConfigurationException>(async () => await service.Value());
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.Cancel("cancelled", cancelled.Token));
                Assert.Equal(0, service.Executions);
            }
        }

        [Fact]
        public async Task Method_override_disable_direct_construction_and_self_calls_obey_proxy_boundary()
        {
            var service = GetRequiredService<ProxyOperations>();
            await service.Run();
            await service.FailBypass();
            Assert.Equal(7, await new ProxyOperations().Run());
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => service.SelfCall());
            Assert.Equal(9, await service.Disabled());
            Assert.Equal(1, await GetRequiredService<DisabledOperations>().Run());
        }

        [Fact]
        public void Invocation_metadata_is_reused_after_registration()
        {
            var method = typeof(ParameterOperations).GetMethod(nameof(ParameterOperations.Marked))!;
            var first = OperationRateLimitingInvocationContextResolver.GetMetadata(typeof(ParameterOperations), method);
            var second = OperationRateLimitingInvocationContextResolver.GetMetadata(typeof(ParameterOperations), method);
            Assert.Same(first, second);
            Assert.Equal(0, first.ParameterIndex);
        }

        [Fact]
        public async Task Inherited_class_and_method_attributes_use_registered_metadata()
        {
            var declared = typeof(BaseOperations).GetMethod(nameof(BaseOperations.Run))!;
            var inherited = typeof(InheritedOperations).GetMethod(nameof(BaseOperations.Run))!;
            Assert.Same(OperationRateLimitingInvocationContextResolver.GetMetadata(typeof(InheritedOperations), inherited),
                OperationRateLimitingInvocationContextResolver.GetMetadata(typeof(InheritedOperations), declared));
            var service = GetRequiredService<InheritedOperations>();
            await service.Run();
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => service.Run());
            await service.Parameter("inherited");
            await Assert.ThrowsAsync<OperationRateLimitExceededException>(() => service.Parameter("inherited"));
            await service.Disabled();
        }

        [Fact]
        public void Exception_code_namespace_maps_to_matching_localization_resource()
        {
            var mappings = GetRequiredService<IOptions<AbpExceptionLocalizationOptions>>().Value.ErrorCodeNamespaceMappings;
            Assert.Equal(typeof(OperationRateLimitingResource), mappings["OperationRateLimiting"]);
            Assert.Equal("OperationRateLimiting:LimitExceeded", OperationRateLimitingErrorCodes.LimitExceeded);
        }

        [Theory]
        [InlineData("en", "Operation policy", "seconds")]
        [InlineData("zh-Hans", "操作策略", "秒")]
        [InlineData("zh-Hant", "操作策略", "秒")]
        [InlineData("vi", "Chính sách", "giây")]
        [InlineData("tr", "işlem", "saniye")]
        [InlineData("sl", "Politika", "sekund")]
        [InlineData("pt-BR", "política", "segundos")]
        [InlineData("pl-PL", "Polityka", "sekund")]
        [InlineData("cs", "Zásada", "sekund")]
        public void Localized_resources_and_formatter_are_registered(string culture, string errorText, string durationText)
        {
            using (CultureHelper.Use(CultureInfo.GetCultureInfo(culture)))
            {
                var localizer = GetRequiredService<IStringLocalizer<OperationRateLimitingResource>>();
                var error = localizer[OperationRateLimitingErrorCodes.LimitExceeded];
                Assert.False(error.ResourceNotFound);
                Assert.Contains(errorText, error.Value);
                Assert.Contains(durationText, GetRequiredService<IOperationRateLimitingFormatter>().Format(TimeSpan.FromSeconds(2.1)));
            }
        }
    }

    public class KeyDto : IHasOperationRateLimitingParameter
    {
        private readonly string? _key;

        public KeyDto(string? key)
        {
            _key = key;
        }

        public string? GetPartitionParameter()
        {
            return _key;
        }
    }

    [OperationRateLimiting("parameter")]
    public class ParameterOperations
    {
        public int Executions { get; private set; }

        public virtual Task Marked([RateLimitingParameter] string key, KeyDto first, KeyDto second)
        {
            return Task.CompletedTask;
        }

        public virtual Task Dto(KeyDto key)
        {
            return Task.CompletedTask;
        }

        public virtual Task Ambiguous(KeyDto first, KeyDto second)
        {
            throw new Exception("must not run");
        }

        public virtual Task Markers([RateLimitingParameter] string first, [RateLimitingParameter] string second)
        {
            throw new Exception("must not run");
        }

        public virtual Task InvalidMarker([RateLimitingParameter] int key)
        {
            throw new Exception("must not run");
        }

        public virtual Task Tokens(CancellationToken first, CancellationToken second)
        {
            throw new Exception("must not run");
        }

        public virtual int Sync()
        {
            throw new Exception("must not run");
        }

        public virtual ValueTask<int> Value()
        {
            throw new Exception("must not run");
        }

        public virtual Task Cancel([RateLimitingParameter] string key, CancellationToken token)
        {
            Executions++;
            return Task.CompletedTask;
        }
    }

    [DisableOperationRateLimiting]
    public class DisabledOperations
    {
        [OperationRateLimiting("unknown")]
        public virtual Task<int> Run()
        {
            return Task.FromResult(1);
        }
    }

    [OperationRateLimiting("one")]
    public class BaseOperations
    {
        public virtual Task Run()
        {
            return Task.CompletedTask;
        }

        [OperationRateLimiting("parameter")]
        public virtual Task Parameter([RateLimitingParameter] string key)
        {
            return Task.CompletedTask;
        }

        [DisableOperationRateLimiting]
        public virtual Task Disabled()
        {
            return Task.CompletedTask;
        }
    }

    public class InheritedOperations : BaseOperations
    {
    }
}
