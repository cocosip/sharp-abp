using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.FileStoring.FileSystem
{
    public class DefaultFilePathCalculator : IFilePathCalculator, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }
        protected IFilePathContextAccessor FilePathContextAccessor { get; }
        protected AbpFileStoringAbstractionsOptions Options { get; }

        public DefaultFilePathCalculator(
            ICurrentTenant currentTenant,
            IFilePathContextAccessor filePathContextAccessor,
            IOptions<AbpFileStoringAbstractionsOptions> options)
        {
            CurrentTenant = currentTenant;
            FilePathContextAccessor = filePathContextAccessor;
            Options = options.Value;
        }

        public virtual string Calculate(FileProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var basePath = fileSystemConfiguration.BasePath;
            var filePath = basePath;
            var builderOptions = Options.FilePathBuilder;
            var context = FilePathContextAccessor.Current;

            EnsureValidFileId(args.FileId);

            if (Options.FilePathStrategy == FilePathGenerationStrategy.DirectFileId)
            {
                if (fileSystemConfiguration.AppendContainerNameToBasePath)
                {
                    filePath = Path.Combine(filePath, args.ContainerName);
                }

                filePath = Path.Combine(filePath, args.FileId);
            }
            else
            {
                // Optional prefix
                var prefix = ResolvePrefixSegment(builderOptions, context);
                if (!string.IsNullOrEmpty(prefix))
                {
                    filePath = Path.Combine(filePath, prefix!);
                }

                // Tenant segment
                if (CurrentTenant.Id == null)
                {
                    filePath = Path.Combine(filePath, builderOptions.HostSegment);
                }
                else
                {
                    var identifier = builderOptions.TenantIdentifierFactory != null
                        ? builderOptions.TenantIdentifierFactory(CurrentTenant.Id.Value, CurrentTenant.Name, context)
                        : CurrentTenant.Id.Value.ToString("D");

                    filePath = Path.Combine(filePath, builderOptions.TenantsSegment, identifier);
                }

                if (fileSystemConfiguration.AppendContainerNameToBasePath)
                {
                    filePath = Path.Combine(filePath, args.ContainerName);
                }

                filePath = Path.Combine(filePath, args.FileId);
            }

            return EnsurePathIsUnderBasePath(basePath, filePath, args.FileId);
        }

        protected virtual string? ResolvePrefixSegment(FilePathBuilderOptions options, FilePathContext? context)
        {
            if (options.PrefixFactory != null)
            {
                return options.PrefixFactory(context);
            }

            if (!string.IsNullOrEmpty(context?.Prefix))
            {
                return context!.Prefix;
            }

            return options.Prefix;
        }

        protected virtual string EnsurePathIsUnderBasePath(string basePath, string filePath, string fileId)
        {
            var comparisonType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            var fullBasePath = Path.GetFullPath(basePath);
            var fullFilePath = Path.GetFullPath(filePath);
            var normalizedBasePath = AppendDirectorySeparator(fullBasePath);

            if (!fullFilePath.StartsWith(normalizedBasePath, comparisonType))
            {
                throw new AbpException($"The file identifier '{fileId}' resolves outside the configured file system base path.");
            }

            return fullFilePath;
        }

        protected virtual string AppendDirectorySeparator(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var lastChar = path[path.Length - 1];
                if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
                {
                    return path;
                }
            }

            return path + Path.DirectorySeparatorChar;
        }

        protected virtual void EnsureValidFileId(string fileId)
        {
            if (Path.IsPathRooted(fileId) || fileId.Contains(":") || ContainsTraversalSegment(fileId))
            {
                throw new AbpException($"The file identifier '{fileId}' must be a relative path without traversal segments.");
            }
        }

        protected virtual bool ContainsTraversalSegment(string path)
        {
            foreach (var segment in path.Split(new[] { '/', '\\' }, StringSplitOptions.None))
            {
                if (segment == "." || segment == "..")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
