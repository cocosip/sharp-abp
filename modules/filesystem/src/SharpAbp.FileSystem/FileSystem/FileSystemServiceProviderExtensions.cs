using FastDFSCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace SharpAbp.FileSystem
{
    /// <summary>依赖注入扩展
    /// </summary>
    public static class FileSystemServiceProviderExtensions
    {
        /// <summary>配置文件存储系统
        /// </summary>
        internal static IServiceProvider ConfigureFileSystem(this IServiceProvider provider, Action<FileSystemOption> configure = null)
        {
            if (configure != null)
            {
                var injectOption = provider.GetService<IOptions<FileSystemOption>>().Value;
                configure?.Invoke(injectOption);
            }

            //查询全部patch
            var fileSystemQuery = provider.GetService<IFileSystemQuery>();

            var storeTypes = AsyncHelper.RunSync(() =>
            {
                return fileSystemQuery.FindStoreTypesAsync();
            });

            //是否对FastDFS做配置
            if (storeTypes.Any(x => x == StoreType.FastDFS))
            {
                provider.ConfigureFastDFSCore();
            }

            return provider;
        }

    }
}
