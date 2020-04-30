using FastDFSCore;
using SharpAbp.FileSystem.FastDFS;
using SharpAbp.FileSystem.Impl;
using SharpAbp.FileSystem.S3;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SharpAbp.FileSystem
{
    /// <summary>文件存储操作相关依赖注入
    /// </summary>
    public static class FileSystemServiceCollectionExtensions
    {

        /// <summary>添加文件存储系统
        /// </summary>
        internal static IServiceCollection AddFileSystem(this IServiceCollection services, Action<FileSystemOption> configure = null)
        {
            if (configure == null)
            {
                configure = o => { };
            }

            services
                .AddS3FileSystem() //S3相关依赖注入
                .AddFastDFSFileSystem() //FastDFS相关依赖
                .Configure<FileSystemOption>(configure)
                .AddSingleton<IFileStore, DefaultFileStore>()
                .AddSingleton<IFileIdGenerator, DefaultFileIdGenerator>()
                .AddTransient<IPatchSelector, DefaultPatchSelector>()
                .AddTransient<IFileSystemQuery, EmptyFileSystemQuery>()
                .AddTransient<IPatchTranslator, DefaultPatchTranslator>();
            return services;
        }

        /// <summary>添加S3存储
        /// </summary>
        internal static IServiceCollection AddS3FileSystem(this IServiceCollection services)
        {
            services
                .AddSingleton<IS3ClientFactory, DefaultS3ClientFactory>()
                .AddTransient<IFileStoreClient, S3FileStoreClient>();
            return services;
        }


        /// <summary>添加FastDFS存储
        /// </summary>
        internal static IServiceCollection AddFastDFSFileSystem(this IServiceCollection services)
        {
            services
                .AddTransient<IFileStoreClient, FastDFSFileStoreClient>()
                .AddFastDFSCore();
            return services;
        }

    }
}
