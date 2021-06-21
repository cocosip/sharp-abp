using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.MapTenancy
{
    public class DefaultMapTenancyConfigurationProvider : IMapTenancyConfigurationProvider, ITransientDependency
    {
        protected AbpMapTenancyOptions Options { get; }
        public DefaultMapTenancyConfigurationProvider(IOptions<AbpMapTenancyOptions> options)
        {
            Options = options.Value;
        }

        [NotNull]
        public virtual Task<MapTenancyConfiguration> GetAsync([NotNull] string code)
        {
            return Task.FromResult(Options.Mappers.GetConfiguration(code));
        }

        [NotNull]
        public virtual Task<MapTenancyConfiguration> GetByMapCodeAsync([NotNull] string mapCode)
        {
            return Task.FromResult(Options.Mappers.GetConfigurationByMapCode(mapCode));
        }
    }
}
