using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.FileStoring
{
    public class DefaultFilePathContextResolver : IFilePathContextResolver, ITransientDependency
    {
        protected IServiceProvider ServiceProvider { get; }

        protected AbpFilePathContextResolveOptions Options { get; }

        public DefaultFilePathContextResolver(
            IServiceProvider serviceProvider,
            IOptions<AbpFilePathContextResolveOptions> options)
        {
            ServiceProvider = serviceProvider;
            Options = options.Value;
        }

        public virtual async Task<FilePathContext?> ResolveAsync()
        {
            var context = new FilePathContextResolveContext(ServiceProvider);
            foreach (var contributor in Options.Contributors)
            {
                await contributor.ResolveAsync(context);
                if (context.Handled)
                {
                    break;
                }
            }

            return context.FilePathContext;
        }
    }
}
